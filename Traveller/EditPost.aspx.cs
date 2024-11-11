using MediaToolkit.Model;
using MediaToolkit.Options;
using MediaToolkit;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace FYP_TravelPlanner.Traveller
{
    public partial class EditPost : System.Web.UI.Page
    {
        protected string postId;
        protected string fileType;
        protected int numImages;

        protected void Page_Load(object sender, EventArgs e)
        {
            postId = Request.QueryString["p"];
            if (!IsPostBack)
            {
                LoadPostData(postId); // Load post data and existing media on initial load
            }
        }

        private void LoadPostData(string postId)
        {
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = "SELECT post_title, post_content, post_permission, file_type, num_image FROM Posts WHERE post_id = @PostID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PostID", postId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtPostTitle.Text = reader["post_title"].ToString();
                            txtPostContent.Text = reader["post_content"].ToString();
                            rblPostPermission.SelectedValue = reader["post_permission"].ToString();
                            fileType = reader["file_type"].ToString();
                            numImages = Convert.ToInt32(reader["num_image"]);
                        }
                    }
                }
            }

            // Load existing media (images or video) into the UI container
            LoadExistingMedia(postId, fileType, numImages);
        }

        private void LoadExistingMedia(string postId, string fileType, int numImages)
        {
            string previewHtml = string.Empty; // Use this to build the HTML for existing media

            if (fileType == "image")
            {
                for (int i = 1; i <= numImages; i++)
                {
                    string imagePath = ResolveUrl($"~/Uploads/Images/{postId}_{i}.jpg");
                    previewHtml += $"<div class='image-preview-container'>" +
                                   $"<img src='{imagePath}' alt='Image {i}' style='width: 100%; height: 100%; object-fit: cover;' />" +
                                   $"<button class='delete-button' onclick=\"removeFile('{imagePath}')\">x</button>" +
                                   $"</div>";
                }
            }
            else if (fileType == "video")
            {
                string videoPath = ResolveUrl($"~/Uploads/Videos/{postId}.mp4");
                string thumbnailPath = ResolveUrl($"~/Uploads/Images/{postId}_1.jpg");

                previewHtml += $"<div class='video-preview-container'>" +
                               $"<video src='{videoPath}' controls style='width: 100%; height: 100%; object-fit: cover;'></video>" +
                               $"<button class='delete-button' onclick=\"removeFile('{videoPath}')\">x</button>" +
                               $"</div>";

                previewHtml += $"<div class='image-preview-container'>" +
                               $"<img src='{thumbnailPath}' alt='Video Thumbnail' style='width: 100%; height: 100%; object-fit: cover;' />" +
                               $"</div>";
            }

            // Assign the generated HTML to the previewLiteral control
            previewLiteral.Text = previewHtml;
        }

        protected void btnUpdatePost_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                // Delete existing files only if new files are uploaded
                DeleteExistingFiles(postId);

                string[] allowedImageExtensions = { ".jpg", ".jpeg", ".png" };
                string[] allowedVideoExtensions = { ".mp4" };
                string firstFileExtension = Path.GetExtension(fileUpload.PostedFiles[0].FileName).ToLower();

                if (Array.Exists(allowedImageExtensions, ext => ext == firstFileExtension))
                {
                    fileType = "image";
                    numImages = HandleMultipleImagesUpload(postId, allowedImageExtensions);
                }
                else if (Array.Exists(allowedVideoExtensions, ext => ext == firstFileExtension))
                {
                    fileType = "video";
                    numImages = 1;
                    HandleVideoUpload(postId);
                }
                else
                {
                    lblMessage.Text = "Invalid file type. Please upload images or a video.";
                    return;
                }
            }

            UpdatePostInDatabase(postId, fileType, numImages);
            Response.Redirect("~/Traveller/PostDetails.aspx?post_id=" + postId);
        }

        private void DeleteExistingFiles(string postId)
        {
            string imagesPath = Server.MapPath("~/Uploads/Images/");
            string videosPath = Server.MapPath("~/Uploads/Videos/");

            if (fileType == "image")
            {
                for (int i = 1; i <= numImages; i++)
                {
                    var imagePath = Path.Combine(imagesPath, $"{postId}_{i}.jpg");
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
            }
            else if (fileType == "video")
            {
                var videoPath = Path.Combine(videosPath, $"{postId}.mp4");
                var thumbnailPath = Path.Combine(imagesPath, $"{postId}_1.jpg");

                if (File.Exists(videoPath)) File.Delete(videoPath);
                if (File.Exists(thumbnailPath)) File.Delete(thumbnailPath);
            }
        }

        private int HandleMultipleImagesUpload(string postId, string[] allowedImageExtensions)
        {
            int imageIndex = 1;

            foreach (HttpPostedFile file in fileUpload.PostedFiles)
            {
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (Array.Exists(allowedImageExtensions, ext => ext == extension))
                {
                    string fileName = $"{postId}_{imageIndex}.jpg";
                    string filePath = Server.MapPath("~/Uploads/Images/" + fileName);
                    file.SaveAs(filePath);
                    imageIndex++;
                }
            }

            return imageIndex - 1;
        }

        private void HandleVideoUpload(string postId)
        {
            string videoFileName = $"{postId}.mp4";
            string videoFilePath = Server.MapPath("~/Uploads/Videos/" + videoFileName);

            fileUpload.PostedFiles[0].SaveAs(videoFilePath);

            string thumbnailFilePath = Server.MapPath("~/Uploads/Images/" + postId + "_1.jpg");
            GenerateVideoThumbnail(videoFilePath, thumbnailFilePath);
        }

        private void GenerateVideoThumbnail(string videoPath, string thumbnailPath)
        {
            var inputFile = new MediaFile { Filename = videoPath };
            var outputFile = new MediaFile { Filename = thumbnailPath };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(1) };
                engine.GetThumbnail(inputFile, outputFile, options);
            }
        }

        private void UpdatePostInDatabase(string postId, string fileType, int numImages)
        {
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = "UPDATE Posts SET post_title = @Title, post_content = @Content, post_permission = @Permission, file_type = @FileType, num_image = @NumImages WHERE post_id = @PostID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", txtPostTitle.Text);
                    cmd.Parameters.AddWithValue("@Content", txtPostContent.Text);
                    cmd.Parameters.AddWithValue("@Permission", rblPostPermission.SelectedValue);
                    cmd.Parameters.AddWithValue("@FileType", fileType);
                    cmd.Parameters.AddWithValue("@NumImages", numImages);
                    cmd.Parameters.AddWithValue("@PostID", postId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
