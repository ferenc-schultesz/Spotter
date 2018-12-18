using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ISIC_Concole_Downloader
{
    class Program
    {
        static string username = "";
        static string password = "";
        static int numOfImages = 0;
        static string imagesPath = "";

        static void Main(string[] args)
        {
            Logger.LogMsg("-----------------------------------------------------------");
            Logger.LogMsg("-----------   ISIC Dataset Downloader    ------------------");
            Logger.LogMsg("-----------------------------------------------------------");
            
            // ------------  GET USER INPUTS TRHOUGH CONSOLE MENU --------------------
            // Login user
            bool loggedIn = false;
            
            while (!loggedIn)
            {
                GetLoginDetails();
                Logger.LogMsg($"{DateTime.Now}: Attempt to Login with {username}");
                loggedIn = APIHelper.Login(username, password);
            }
            Logger.LogMsg("Login successful");

            // Get user input for download location
            GetImagesPath();
            // Get user input for number of images to download
            GetNumOfImages();
                        
            // Get images meta data
            Logger.LogMsg("");
            Logger.LogMsg("----------------   Getting Image metadata    --------------");
            var lImageMetadata = APIHelper.GetImagesMetadata(numOfImages);
            // Save images' metadata to CSV
            try
            {
                CreateCSV(lImageMetadata);
            }
            catch (Exception ex)
            {
                Logger.LogErr("Failed to save metadata: " + ex.Message);
                Logger.LogMsg("");
            }
            Logger.LogMsg($"Metadata downloaded for {lImageMetadata.Count.ToString()} images");
            // Filter out the already downloaded images
            lImageMetadata = FilterDownloadedImages(lImageMetadata);

            // Download images
            int counter = 0;
            foreach (Imagemetadata image in lImageMetadata)
            {
                APIHelper.DownloadImage(image._id, image.name, imagesPath);
                counter++;
                if (counter % 50 == 0)
                {
                    Logger.LogMsg($"{DateTime.Now}: {counter} images have been downloaded");
                }

            }
            Logger.LogMsg($"{lImageMetadata.Count.ToString()} images downloaded");

            Console.WriteLine("------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Download finished");
            Console.Write("Close the window to exit.");
            Console.ReadLine();
        }

        public static void CreateCSV(List<Imagemetadata> lImageMetadata)
        {
            using (var w = new StreamWriter(imagesPath + "\\" + "ImagesMetadata.csv"))
            {
                string header = @"_id,_modelType,name,created,creator_id,creator_name,updated,dataset_accessLevel,dataset_id,dataset_license,dataset_name,dataset_updated,meta_acquisition_image_type,meta_acquisition_pixelsX,meta_acquisition_pixelsY,meta_clinical_age_approx,meta_clinical_benign_malignant,meta_clinical_diagnosis,meta_clinical_diagnosis_confirm_type,meta_clinical_melanocytic,meta_clinical_sex,notes_reviewed_accepted,notes_reviewed_time,notes_reviewed_userId, notes_tags";
                w.WriteLine(header);
                w.Flush();
                foreach (Imagemetadata img in lImageMetadata)
                {
                    // Build notes tags
                    string tags = "";
                    foreach (string tag in img.notes.tags)
                    {
                        tags += tag + "/";
                    }
                    // Replaces needed to avoic having commas in string and put data in new column
                    if (!string.IsNullOrEmpty(tags))
                    {
                        tags = tags.Replace(",", "");
                    }
                    if (!string.IsNullOrEmpty(img.dataset.description))
                    {
                        img.dataset.description = img.dataset.description.Replace(",", "");
                    }

                    if (!string.IsNullOrEmpty(img.meta.clinical.benign_malignant))
                    {
                        img.meta.clinical.benign_malignant = img.meta.clinical.benign_malignant.Replace(",", "");
                    }

                    if (!string.IsNullOrEmpty(img.meta.clinical.diagnosis))
                    {
                        img.meta.clinical.diagnosis = img.meta.clinical.diagnosis.Replace(",", "");
                    }

                    
                    var line = $"{img._id.ToString()},{img._modelType},{img.name},{img.created},{img.creator._id},{img.creator.name},{img.updated},{img.dataset._accessLevel},{img.dataset._id},{img.dataset.license},{img.dataset.name},{img.dataset.updated},{img.meta.acquisition.image_type},{img.meta.acquisition.pixelsX},{img.meta.acquisition.pixelsY},{img.meta.clinical.age_approx},{img.meta.clinical.benign_malignant.Replace(",", "")},{img.meta.clinical.diagnosis.Replace(",", "")},{img.meta.clinical.diagnosis_confirm_type},{img.meta.clinical.melanocytic},{img.meta.clinical.sex},{img.notes.reviewed.accepted},{img.notes.reviewed.time},{img.notes.reviewed.userId},{tags.Replace(",", "")}";

                    line = line.Replace("\n", "");
                    w.WriteLine(line);
                    w.Flush();
                }
            }
        }

        public static void GetLoginDetails()
        {
            //Get Login details
            Console.Write("Enter ISIC username: ");
            username = Console.ReadLine();
            Console.Write("Enter ISIC password: ");
            // Clear password variable in case many login attempts 
            password = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                // Backspace should not work when password is empty
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            // Stops Receving Keys Once Enter is Pressed
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine("");
        }

       
        static void GetNumOfImages()
        {
            // Get number of images
            bool valid = false;
            do
            {
                Console.Write("How many images would you like to download (leave empty for maximum): ");
                string tempInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(tempInput))
                {
                    int tempNum;
                    int.TryParse(tempInput, out tempNum);
                    if (tempNum != 0)
                    {
                        numOfImages = tempNum;
                        valid = true;
                    }
                    else
                    {
                        Console.WriteLine("String is not allowed, please enter an integer!");
                    }
                }
                else
                {
                    numOfImages = 100000;
                    Console.WriteLine("Maximum number of images will be downloaded, currently on 08/07/2018 there are 23906 images in ISIC dataset.");
                    valid = true;
                }
            }
            while (!valid);
        }

        static void GetImagesPath()
        {
            // Get image path
            bool pathValid = false;
            do
            {
                Console.Write("Path where images were or will be downloaded: ");
                string tempInput = Console.ReadLine();
                if (!string.IsNullOrEmpty(tempInput))
                {
                    if (Directory.Exists(tempInput))
                    {
                        pathValid = true;
                        imagesPath = tempInput;
                    }
                    else
                    {
                        Console.WriteLine($"{tempInput} is nt a valid path, or direcotry does not exist!");
                    }
                }
                else
                {
                    imagesPath = "c:/DATA/";
					Console.WriteLine("Default path selected: 'c:/DATA/'");
                    pathValid = true;
                }
            }
            while (!pathValid);
        }

        static List<Imagemetadata> FilterDownloadedImages(List<Imagemetadata> oldList)
        {
            //  Get rid of images, already downloaded
            List<Imagemetadata> finalList = new List<Imagemetadata>();
            List<string> lDownloaded = new List<string>();
            foreach (string s in Directory.GetFiles(imagesPath, "*.jpg").Select(Path.GetFileName))
            {
                lDownloaded.Add(s.Replace(".jpg", ""));
            }
            try
            {
                foreach (Imagemetadata img in oldList)
                {
                    bool remove = false;
                    foreach (string downloded in lDownloaded)
                    {
                        if (img.name == downloded)
                        {
                            remove = true;
                            break;
                        }
                    }
                    if (!remove)
                    {
                        finalList.Add(img);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogMsg("Failed to filter list with already downloaded images");
            }
            return finalList;
        }
    }
}
