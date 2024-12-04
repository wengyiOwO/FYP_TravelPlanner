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
            if (Session["account_id"] == null)
            {
                Response.Redirect("~/Login.aspx");
            }

            postId = Request.QueryString["p"];
            if (!IsPostBack)
            {
                LoadPostData(postId);
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

            LoadExistingMedia(postId, fileType, numImages);
        }

        private void LoadExistingMedia(string postId, string fileType, int numImages)
        {
            string previewHtml = string.Empty; 

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

                previewHtml += $"<div class='video-preview-container'>" +
                               $"<video src='{videoPath}' controls style='width: 100%; height: 100%; object-fit: cover;'></video>" +
                               $"<button class='delete-button' onclick=\"removeFile('{videoPath}')\">x</button>" +
                               $"</div>";
            }

            previewLiteral.Text = previewHtml;
        }

        protected void btnUpdatePost_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                DeleteExistingFiles(postId);

                string[] allowedImageExtensions = { ".jpg", ".jpeg", ".png" };
                string[] allowedVideoExtensions = { ".mp4", ".mov", ".avi" };

                long maxVideoSize = 25 * 1024 * 1024; //1GB

                int maxImageCount = 9;

                if (fileUpload.PostedFiles.Count > maxImageCount)
                {
                    lblMessage.Text = $"You can only upload {maxImageCount} images.";
                    lblMessage.Visible = true;
                    return;
                }

                string firstFileExtension = Path.GetExtension(fileUpload.PostedFiles[0].FileName).ToLower();

                if (Array.Exists(allowedImageExtensions, ext => ext == firstFileExtension))
                {
                    //image
                    fileType = "image";
                    numImages = fileUpload.PostedFiles.Count;
                    if (numImages > maxImageCount)
                    {
                        lblMessage.Text = $"You can only upload {maxImageCount} images.";
                        lblMessage.Visible = true;
                        return;
                    }

                    numImages = HandleMultipleImagesUpload(postId, allowedImageExtensions);
                }
                else if (Array.Exists(allowedVideoExtensions, ext => ext == firstFileExtension))
                {
                    //video
                    fileType = "video";

                    if (fileUpload.PostedFiles.Count > 1)
                    {
                        lblMessage.Text = "You can only upload one video.";
                        lblMessage.Visible = true;
                        return;
                    }

                    HttpPostedFile videoFile = fileUpload.PostedFiles[0];
                    if (videoFile.ContentLength > maxVideoSize)
                    {
                        lblMessage.Text = "Video size must be less than 1GB.";
                        lblMessage.Visible = true;
                        return;
                    }

                    numImages = 1;
                    HandleVideoUpload(postId);
                }
                else
                {
                    lblMessage.Text = "Invalid file type. The system only allow for .jpg, .jpeg, .png, .gif, .mp4, .mov, .avi";
                    lblMessage.Visible = true;
                    return;
                }
            }
            else
            {
                // No files uploaded, retain existing
                string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(strCon))
                {
                    conn.Open();
                    string query = "SELECT file_type, num_image FROM Posts WHERE post_id = @PostID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PostID", postId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                fileType = reader["file_type"].ToString();
                                numImages = Convert.ToInt32(reader["num_image"]);
                            }
                        }
                    }
                }
            }

            UpdatePostInDatabase(postId, fileType, numImages);
            successPanel.Visible = true;

            string redirectScript = $@"
            <script type='text/javascript'>
                setTimeout(function() {{
                    window.location.href = 'PostDetails.aspx?post_id={postId}';
                }}, 2000);
            </script>";

            ClientScript.RegisterStartupScript(this.GetType(), "RedirectScript", redirectScript);
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
            int imageCount = 0;
            int imageIndex = 1;

            foreach (HttpPostedFile file in fileUpload.PostedFiles)
            {
                string extension = Path.GetExtension(file.FileName).ToLower();

                if (Array.Exists(allowedImageExtensions, ext => ext == extension))
                {
                    string fileName = $"{postId}_{imageIndex}.jpg";
                    string filePath = Server.MapPath("~/Uploads/Images/" + fileName);

                    string directoryPath = Server.MapPath("~/Uploads/Images/");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    try
                    {
                        file.SaveAs(filePath);
                        imageCount++;
                        imageIndex++;
                    }
                    catch (Exception ex)
                    {
                        lblMessage.Text = "Error saving image: " + ex.Message;
                    }
                }
            }

            return imageCount;
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
