using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;

namespace FYP_TravelPlanner.Traveller
{
    public partial class CreatePost : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string GenerateNextPostID()
        {
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                string lastID = GetLastPostIDFromDatabase(conn);
                string nextID = IncrementPostID(lastID);
                return nextID;
            }
        }

        private string GetLastPostIDFromDatabase(SqlConnection connection)
        {
            string query = "SELECT TOP 1 post_id FROM Posts ORDER BY post_id DESC";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                object result = command.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

        private string IncrementPostID(string lastID)
        {
            if (string.IsNullOrEmpty(lastID))
            {
                return "PS000001";
            }

            int numericPart = int.Parse(lastID.Substring(2)) + 1;
            string nextID = $"PS{numericPart:D6}";

            while (PostIDExistsInDatabase(nextID))
            {
                numericPart++;
                nextID = $"PS{numericPart:D6}";
            }

            return nextID;
        }

        private bool PostIDExistsInDatabase(string idToCheck)
        {
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Posts WHERE post_id = @ID";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@ID", idToCheck);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        protected void btnCreatePost_Click(object sender, EventArgs e)
        {
            string postId = GenerateNextPostID();

            if (fileUpload.HasFile)
            {
                string[] allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string[] allowedVideoExtensions = { ".mp4", ".mov", ".avi" };

                string fileType = "";
                int numImages = 0;
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
                    if (fileUpload.PostedFiles.Count > 1)
                    {
                        lblMessage.Text = "You can only upload one video.";
                        return;
                    }

                    HandleVideoUpload(postId);
                }
                else
                {
                    lblMessage.Text = "Invalid file type. Please upload images or a video.";
                    return;
                }

                AddPostToDatabase(postId, fileType, numImages);
                Response.Redirect("~/Traveller/PostDetails.aspx?post_id=" + postId);
            }
            else
            {
                lblMessage.Text = "Please upload an image or video.";
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
                    string fileName = $"{postId}_{imageIndex}{extension}";
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
            string fileName = $"{postId}.mp4";
            string videoFilePath = Server.MapPath("~/Uploads/Videos/" + fileName);

            string videoDirectory = Server.MapPath("~/Uploads/Videos/");
            if (!Directory.Exists(videoDirectory))
            {
                Directory.CreateDirectory(videoDirectory);
            }

            try
            {
                fileUpload.PostedFiles[0].SaveAs(videoFilePath);

                string thumbnailPath = Server.MapPath("~/Uploads/Images/" + postId + "_1.jpg");
                GenerateVideoThumbnail(videoFilePath, thumbnailPath);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error saving video: " + ex.Message;
            }
        }

        private void GenerateVideoThumbnail(string videoPath, string thumbnailPath)
        {
            var inputFile = new MediaFile { Filename = videoPath };
            var outputFile = new MediaFile { Filename = thumbnailPath };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile); // Retrieve metadata, e.g., duration

                // Generate a thumbnail at the 1-second mark
                var options = new ConversionOptions { Seek = TimeSpan.FromSeconds(1) };
                engine.GetThumbnail(inputFile, outputFile, options);
            }
        }

        private void AddPostToDatabase(string postId, string fileType, int numImages)
        {
            string strCon = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(strCon))
            {
                conn.Open();

                string strInsert = "INSERT INTO Posts (post_id, account_id, post_date, post_title, post_content, post_status, post_permission, file_type, num_image) " +
                                   "VALUES (@PostID, @AccountID, @PostDate, @PostTitle, @PostContent, @PostStatus, @PostPermission, @FileType, @NumImage)";

                using (SqlCommand cmdInsert = new SqlCommand(strInsert, conn))
                {
                    cmdInsert.Parameters.AddWithValue("@PostID", postId);
                    cmdInsert.Parameters.AddWithValue("@AccountID", "AC0521");
                    cmdInsert.Parameters.AddWithValue("@PostDate", DateTime.Now);
                    cmdInsert.Parameters.AddWithValue("@PostTitle", txtPostTitle.Text);
                    cmdInsert.Parameters.AddWithValue("@PostContent", txtPostContent.Text);
                    cmdInsert.Parameters.AddWithValue("@PostStatus", "Posted");
                    cmdInsert.Parameters.AddWithValue("@PostPermission", rblPostPermission.SelectedValue);
                    cmdInsert.Parameters.AddWithValue("@FileType", fileType);
                    cmdInsert.Parameters.AddWithValue("@NumImage", numImages);

                    cmdInsert.ExecuteNonQuery();
                }
            }
        }
    }
}