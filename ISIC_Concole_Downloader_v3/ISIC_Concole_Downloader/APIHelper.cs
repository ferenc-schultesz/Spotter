using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ISIC_Concole_Downloader
{
    class APIHelper
    {
        // Base URI for ISIC web API
        static string API = @"https://isic-archive.com:443/api/v1/";
        // Variable for storing the cookie after successful login
        public static Cookie myCookie;

        /// <summary>
        /// Function to login to ISIC Api
        /// </summary>
        /// <param name="username, registered at ISIC archieve"></param>
        /// <param name="username's password"></param>
        /// <returns></returns>
        public static bool Login(string username, string password)
        {
            // Building http request header with basic authentication
            var request = (HttpWebRequest)WebRequest.Create(API + @"user/authentication");
            string authInfo = username + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }
                    // grab the response
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                    // response content has the cookie, that will be used for the rest of the requests
                    myCookie = new Cookie("GirderCookie", response.ToString());
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogErr(@"Login failed - " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Function to get metadata for the ISIC images
        /// </summary>
        /// <param name="Number of images to get the metadata for (default 100000)"></param>
        /// <returns></returns>
        public static List<Imagemetadata> GetImagesMetadata(int numOfImages = 100000)
        {
            // Get the list of images (no metadata yet, just id, name etc..)
            // Building http request, using the cookie for authentication
            var request = (HttpWebRequest)WebRequest.Create(API + $"image?limit={numOfImages.ToString()}&offset=0&sort=name&sortdir=1");
            request.CookieContainer = new CookieContainer();
            Uri uri = new Uri("https://isic-archive.com:443/api/v1/");
            request.CookieContainer.Add(uri, myCookie);
            var responseValue = string.Empty;
            Logger.LogMsg($"{DateTime.Now}: Getting list of images");
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }
            }
            // Deserializing json to list of objects
            List<Imagemetadata> lImgMeta = new List<Imagemetadata>();
            lImgMeta = JsonConvert.DeserializeObject<List<Imagemetadata>>(responseValue);

            // Now getting the metadata for each image with a separate API call (takes hours with max number of images)
            Logger.LogMsg($"{DateTime.Now}: Getting metadata for images");
            Console.WriteLine($"{DateTime.Now}: Start getting metadata for images.");
            int counter = 0;
            for (int i = 0; i < lImgMeta.Count; ++i)
            {
                // GET Information of images
                request = (HttpWebRequest)WebRequest.Create(API + @"image/" + lImgMeta[i]._id);
                request.CookieContainer = new CookieContainer();
                uri = new Uri("https://isic-archive.com:443/api/v1/");
                request.CookieContainer.Add(uri, myCookie);
                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                            throw new ApplicationException(message);
                        }
                        // grab the response
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseValue = reader.ReadToEnd();
                                }
                        }
                    }

                    // converting the json to Imagemetadata object
                    var tempImg = JsonConvert.DeserializeObject<Imagemetadata>(responseValue);
                    // The lImgMeta was already a list of Imagemetadata, but now we add the rest of the properties to each object in the list
                    lImgMeta[i]._id = tempImg._id;
                    lImgMeta[i]._modelType = tempImg._modelType;
                    lImgMeta[i].created = tempImg.created;
                    lImgMeta[i].creator = tempImg.creator;
                    lImgMeta[i].dataset = tempImg.dataset;
                    lImgMeta[i].meta = tempImg.meta;
                    lImgMeta[i].name = tempImg.name;
                    lImgMeta[i].notes = tempImg.notes;
                    lImgMeta[i].updated = tempImg.updated;
                }
                catch (Exception ex)
                {
                    Logger.LogErr("Connection lost: " + lImgMeta[i].name + " Id: " + lImgMeta[i]._id.ToString());
                }
                // Message for logging and console after every 50 images, so user can monitor the progress
                counter++;
                if (counter % 50 == 0)
                {
                    Logger.LogMsg($"{DateTime.Now}: {counter} images' metadata done");
                }
            }
            return lImgMeta;
        }

        /// <summary>
        /// Function that calls the image/id/download end point on ISIC api to download an actualy (single) image
        /// </summary>
        /// <param name="ISIC id of an image"></param>
        /// <param name="ISIC name of an image"></param>
        public static void DownloadImage (string id, string name, string path)
        {
            // Building http request, using the cookie for authentication
            var request = (HttpWebRequest)WebRequest.Create(API + $"image/{id}/download");
            request.CookieContainer = new CookieContainer();
            Uri uri = new Uri("https://isic-archive.com:443/api/v1/");
            request.CookieContainer.Add(uri, myCookie);

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream output = File.OpenWrite(path + "/" + name + ".jpg"))
                    using (Stream input = response.GetResponseStream())
                    {
                        input.CopyTo(output);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErr(@"Downloading dataset: " + id + @" failed - " + ex.Message);
                throw (ex);
            }
        }

        // NOT USED 
        /// <summary>
        /// Function to download a whole dataset from ISIC. Currently not used, because the ISIC api was upgraded and it does not provide an endpoint to download whole datasets.
        /// </summary>
        /// <param name="datasetId's Id"></param>
        public static void DownloadDataset(string datasetId)
        {
            // Building http request, using the cookie for authentication
            var request = (HttpWebRequest)WebRequest.Create(API + "image/download?datasetId=" + datasetId);
            request.CookieContainer = new CookieContainer();
            Uri uri = new Uri("https://isic-archive.com:443/api/v1/dataset");
            request.CookieContainer.Add(uri, myCookie);

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream output = File.OpenWrite(datasetId + ".zip"))
                    using (Stream input = response.GetResponseStream())
                    {
                        input.CopyTo(output);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogErr(@"Downloading dataset: " + datasetId + @" failed - " + ex.Message);
                throw (ex);
            }
        }

        /// <summary>
        /// Function to get all metadata to every dataset from ISIC. Currently not used
        /// </summary>
        /// <param name="Number of datasets to get the metadata for (default: 50)"></param>
        /// <returns></returns>
        public static List<Datasetmetadata> GetDatasetsMetadata(int numOfDatasets = 50)
        {
            // Building http request, using the cookie for authentication
            var request = (HttpWebRequest)WebRequest.Create($"https://isic-archive.com:443/api/v1/dataset?limit={numOfDatasets}&offset=0&sort=name&sortdir=1");

            request.CookieContainer = new CookieContainer();
            Uri uri = new Uri("https://isic-archive.com:443/api/v1/");
            request.CookieContainer.Add(uri, myCookie);

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var responseValue = string.Empty;
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }
                    // grab the response
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                    return JsonConvert.DeserializeObject<List<Datasetmetadata>>(responseValue);
                }
            }
            catch (Exception ex)
            {
                Logger.LogErr(@"Fetching dataset information failed - " + ex.Message);
                return null;
            }
        }
    }
}
