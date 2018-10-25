using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_NTA_.Entity;
using UWP_NTA_.Service;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_NTA_.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private Member currentMember;
        private static StorageFile file;
        private static string UploadUrl;
        public HomePage()
        {
            GetUploadUrl();
            this.currentMember = new Member();
            this.InitializeComponent();
        }
        private async void Handle_Signup(object sender, RoutedEventArgs e)
        {
            var flag = 0;
            if(this.Email.Text == null)
            {
                Email_Error.Text = "*";
                flag = 1;
            }
            if (this.FirstName == null)
            {
                FirstName_Error.Text = "*";
                flag = 1;
            }
            if (this.LastName.Text == null)
            {
                LastName_Error.Text = "*";
                flag = 1;
            }
            if (this.Phone.Text == null)
            {
                Phone_Error.Text = "*";
                flag = 1;
            }

            if(flag == 0)
            {
                currentMember.firstName = this.FirstName.Text;
                currentMember.lastName = this.LastName.Text;
                currentMember.email = this.Email.Text;
                currentMember.password = this.Password.Password.ToString();
                currentMember.avatar = this.ImageUrl.Text;
                currentMember.phone = this.Phone.Text;
                currentMember.address = this.Address.Text;
                var httpResponseMessage = ApiHandle.Sign_Up(this.currentMember);
                if (httpResponseMessage.Result.StatusCode == HttpStatusCode.Created)
                {
                    Debug.WriteLine("Success");
                }
                else
                {
                    //var errorJson = await httpResponseMessage.Result.Content.ReadAsStringAsync();
                    //ErrorResponse errResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorJson);
                    //foreach (var errorField in errResponse.error.Keys)
                    //{
                    //    TextBlock textBlock = this.FindName(errorField) as TextBlock;
                    //    textBlock.Text = errResponse.error[errorField];
                    //}
                }

            } else
            {
                Register_error.Text = "field (*) can not be null";
            }
            
        }
        private async void Capture_Photo(object sender, RoutedEventArgs e)
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
            file = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (file == null)
            {
                // User cancelled photo capture
                return;
            }
            HttpUploadFile(UploadUrl, "myFile", "image/png");
        }
        private static async void GetUploadUrl()
        {
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();
            Uri requestUri = new Uri("https://1-dot-backup-server-002.appspot.com/get-upload-token");
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            Debug.WriteLine(httpResponseBody);
            UploadUrl = httpResponseBody;
        }
        public async void HttpUploadFile(string url, string paramName, string contentType)
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";

            Stream rs = await wr.GetRequestStreamAsync();
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string header = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", paramName, "path_file", contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            // write file.
            Stream fileStream = await file.OpenStreamForReadAsync();
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);

            WebResponse wresp = null;
            try
            {
                wresp = await wr.GetResponseAsync();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                //Debug.WriteLine(string.Format("File uploaded, server response is: @{0}@", reader2.ReadToEnd()));
                //string imgUrl = reader2.ReadToEnd();
                Uri u = new Uri(reader2.ReadToEnd(), UriKind.Absolute);
                Debug.WriteLine(u.AbsoluteUri);
                ImageUrl.Text = u.AbsoluteUri;
                MyAvatar.Source = new BitmapImage(u);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error uploading file", ex.StackTrace);
                Debug.WriteLine("Error uploading file", ex.InnerException);
                if (wresp != null)
                {
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
        }
        private void Select_Gender(object sender, RoutedEventArgs e)
        {
            RadioButton radioGender = sender as RadioButton;
            currentMember.gender = int.Parse(radioGender.Tag.ToString());

        }
        private void Change_Birthday(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            currentMember.birthday = sender.Date.Value.ToString("yyyy-MM-dd");
        }

        private void Reset_form(object sender, RoutedEventArgs e)
        {
            Email.Text = "";
            Password.Password = "";
            FirstName.Text = "";
            LastName.Text = "";
            ImageUrl.Text = "";
            Phone.Text = "";
            Address.Text = "";
        }
        

        private async void Log_in(object sender, RoutedEventArgs e)
        {
            var httpResponseMessage = ApiHandle.Sign_In(Username.Text, Password_login.Password);

            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
            {
                // save file...
                Debug.WriteLine(responseContent);
                // Doc token
                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

                // Luu token
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync("token.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, responseContent);
                Debug.WriteLine("login success!");

            }
            else
            {
                // Xu ly loi.
                //errorresponse errorobject = jsonconvert.deserializeobject<errorresponse>(responsecontent);
                //if (errorobject != null && errorobject.error.count > 0)
                //{
                //    foreach (var key in errorobject.error.keys)
                //    {
                //        var textmessage = findname(key);
                //        if (textmessage == null)
                //        {
                //            continue;
                //        }
                //        textblock textblock = textmessage as textblock;
                //        textblock.text = errorobject.error[key];
                //        textblock.visibility = visibility.visible;
                //    }
                //}
                //Debug.WriteLine(errorObject);
            }
        }
    }
}
