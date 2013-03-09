using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using SmugMugModel;
using System.Net;

namespace SmugMugTest
{
    class Program
    {

        static void img_UploadProgress(object sender, UploadEventArgs e)
        {
            Console.WriteLine("{1} - {0,5:N}", e.PercentComplete * 100, e.FileName);
        }

        static void img_UploadCompleted(object sender, EventArgs e)
        {
            Console.WriteLine("Upload complete");
        }

        static string RandomString(int size)
        {
            Random myRandom = new Random();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                builder.Append((char)myRandom.Next('A', 'Z' + 1));
            }
            return builder.ToString();
        }





        static void Main(string[] args)
        {
            Album myAlbumTitleOnly = null;
            Category myCategory = null;
            SubCategory mySubCategory = null;
            SubCategory mySubCategoryNew = null;
            Album myAlbumInCategory = null;
            Album myAlbumInSubCategory = null;
            Album myAlbumInCategory2 = null;
            AlbumTemplate myAlbumTemplate = null;
            List<Image> myImageList = new List<Image>();
            Watermark myWatermark1 = null;
            Watermark myWatermark2 = null;
            Image myImage2 = null;
            Image myImage3 = null;
            AlbumTemplate at = null;
            List<Album> myAlbumList = null;

            Console.WriteLine("Connect to the site by logging in");
            Site mySite = new Site();
            Site.Proxy = WebRequest.DefaultWebProxy;
            //Console.Write("Give username: ");
            //String userName = Console.ReadLine();
            //Console.Write("Give password: ");
            //String password = Console.ReadLine();
            String userName = "h1591482@rtrtr.com";
            String password = "Test1234";
            var user = mySite.Login(userName, password);

            Console.WriteLine(user.DisplayName);

            ////myAlbumList = user.GetAlbums(true);
            ////myImageList = myAlbumList[0].GetImages();

            try
            {
                // Test featured albums - for the moment they only return an album with an id and key (heavy returns the same as basic)
                Console.WriteLine("Featured Albums:");
                var myFeatured = user.GetFeaturedAlbums();
                if ((myFeatured != null) && (myFeatured.Albums != null))
                {
                    foreach (var item in myFeatured.Albums)
                    {
                        Console.WriteLine(item.Key);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try
            {
                // Get a list of themes for the user
                Console.WriteLine("List of themes for the user: ");
                var myThemesList = user.GetThemes();
                foreach (var x in myThemesList)
                {
                    Console.WriteLine(x.Name + " - " + x.Type);
                }
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {

                // Get the Styles from the site
                Console.WriteLine("The styles: ");
                var myTemplatesList = mySite.StylesGetTemplates();
                foreach (var x in myTemplatesList)
                {
                    Console.WriteLine(x.Name);
                }
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }



            // General use
            try
            {
                // Create an album only with title (if the category isn't specified, the default one is "Other")
                myAlbumTitleOnly = user.CreateAlbum("TestAlbumTitleOnly");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                // Create a category
                myCategory = user.CreateCategory("TestCategory");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                // Create a subcategory in a category
                mySubCategory = myCategory.CreateSubCategory("TestSubCategory");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                // Creating an album in a category
                myAlbumInCategory = myCategory.CreateAlbum("TestAlbumInCategory");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                // Creating an album in a subcategory
                myAlbumInSubCategory = mySubCategory.CreateAlbum("TestAlbumInSubCategory");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            try
            {
                // Get a hierarchical album tree for the user
                Console.WriteLine("Album tree for the user: ");
                var list = user.GetTree(true);
                foreach (var x in list)
                {
                    var alb = x.Albums;
                    if (alb != null)
                    {
                        foreach (var y in alb)
                        {
                            Console.WriteLine("{0}-{1}", x.Name, y.Title);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();




            try
            {
                Console.WriteLine("Test the community functions");
                //// Join the NaturePhotographers community
                //Community myCommunity = new Community();
                //myCommunity.id = 456;
                //myCommunity.Join();
                //// Another method to join a community - this time the Birds and flowers-flowers-and-more-flowers communities
                //user.JoinCommunity(356);
                //user.JoinCommunity(169);
                // View the communities you are joint to
                var myCommunitiesList = user.GetCommunities();
                if (myCommunitiesList != null)
                {
                    foreach (var x in myCommunitiesList)
                    {
                        Console.WriteLine(x.Name + ' ' + x.id);
                    }
                }
                //// Leave the NaturePhotographers community
                //myCommunity.Leave();
                //// Leave the Birds communities
                //user.LeaveCommunity(356);
                //// Leave all communities
                //user.LeaveAllCommunities();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            Console.WriteLine("Test the family functions");
            // Adding three smugmug heroes as family (for test)
            try
            {
                user.AddFamily("baldy");
                user.AddFamily("markabby");
                user.AddFamily("beanland");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // Get a list of your family and display it
            List<Family> myFamilyList = null;
            try
            {
                myFamilyList = user.GetFamily();
                foreach (var x in myFamilyList)
                {
                    Console.WriteLine(x.DisplayName + ' ' + x.URL);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                // Removing someone from family
                user.RemoveFamily("baldy");
                // Remove all family
                user.RemoveAllFamily();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();



            Console.WriteLine("Test the friends functions");
            try
            { // Adding three smugmug heroes as friends
                user.AddFriend("baldy");
                user.AddFriend("markabby");
                user.AddFriend("beanland");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            try
            {
                // Get a list of your friends and display it
                var myFriendsList = user.GetFriends();
                if (myFriendsList != null)
                {
                    foreach (var x in myFriendsList)
                    {
                        Console.WriteLine(x.DisplayName + ' ' + x.URL);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            try
            {
                // Removing someone from friends
                user.RemoveFriend("baldy");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            try
            {
                // Remove all friends
                user.RemoveAllFriends();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            

            Console.WriteLine("Test the fans functions");
            try
            {
                var myFans = user.GetFans();
                if ((myFans != null) && (myFans.Count > 0))
                {
                    foreach (var item in myFans)
                    {
                        Console.WriteLine(item.DisplayName);
                    }                    
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();


            Console.WriteLine("Test the sharegroup methods");
            ShareGroup myShareGroup = null;
            ShareGroup myShareGroupWithDescription = null;
            try
            {
                // Create a sharegroup without description
                myShareGroup = user.CreateShareGroup("TestShareGroup");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                // Create a sharegroup with description 
                myShareGroupWithDescription = user.CreateShareGroup("TestShareGroupWithDescription", "Something for testing");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // Get a list of sharegroups and display it
            //////var myShareGroupsList = user.GetShareGroups(true);
            //////foreach (var x in myShareGroupsList)
            //////{
            //////    Console.WriteLine(x.Name + ' ' + x.Description + ' ' + x.URL);
            //////}

            try
            {
                // Retrieving info about a sharegroup from the site by creating a new sharegroup in which it will be stored
                myShareGroup = myShareGroup.GetInfo();
                Console.WriteLine(myShareGroup.Name + ' ' + myShareGroup.Tag);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                // Retrieving info about a sharegroup from the site by populating the current object
                myShareGroupWithDescription.PopulateShareGroupWithInfoFromSite();
                Console.WriteLine(myShareGroupWithDescription.Name + ' ' + myShareGroupWithDescription.Tag);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            try
            {
                // Add an album to a sharegroup
                myShareGroup.AddAlbum(myAlbumTitleOnly);
                Console.WriteLine("Number of albums on sharegroup: " + myShareGroup.AlbumCount);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // Get the albums that a sharegroup has
            //////var myShareGroupAlbums = myShareGroup.GetAlbums();
            //////foreach (var x in myShareGroupAlbums)
            //////{
            //////    Console.WriteLine(x.Title);
            //////}

            try
            {
                // Remove an album from a sharegroup
                myShareGroup.RemoveAlbum(myAlbumTitleOnly);
                Console.WriteLine(myShareGroup.AlbumCount);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            try
            {
                //Deleting a sharegroup
                myShareGroup.Delete();
                myShareGroupWithDescription.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            Console.WriteLine("Test the category and subcategory methods");
            try
            {
                List<Category> myCategoriesList = null;
                // Retrieves a list of categories for the user
                myCategoriesList = user.GetCategories();
                foreach (var x in myCategoriesList)
                {
                    Console.WriteLine(x.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Renaming a category
            //////myCategory.Rename("TestCategoryRenamed");
            //////Console.WriteLine("TestCategory is renamed to " + myCategory.Name);

            // Create a subcategory within a category
            try
            {
                mySubCategoryNew = myCategory.CreateSubCategory("TestSubCategoryNew");
                Console.WriteLine(mySubCategoryNew.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Get the subcategories for a category
            try
            {
                var mySubCategoriesList = myCategory.GetSubCategories();
                Console.WriteLine(myCategory.Name + " - " + myCategory.SubCategories.Count);
                foreach (var x in mySubCategoriesList)
                {
                    Console.WriteLine(x.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Get all the subcategories that the user has
            try
            {
                var mySubCategoriesListAll = user.GetAllSubCategories();
                foreach (var x in mySubCategoriesListAll)
                {
                    Console.WriteLine(x.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Rename a subcategory
            try
            {
                mySubCategoryNew.Rename("TestSubCategoryNewChangedName");
                Console.WriteLine("TestSubCategoryNew is renamed to " + mySubCategoryNew.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Deleting a subcategory
            try
            {
                mySubCategoryNew.Delete();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }               
            
            // Creating an album in a category
            try
            {
                Console.WriteLine(myCategory.Albums.Count);
                myAlbumInCategory2 = myCategory.CreateAlbum("TestAlbumInCategory2");
                Console.WriteLine();
                Console.WriteLine(myCategory.Albums.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try
            {
                // Deleting that album
                myAlbumInCategory2.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();



            Console.WriteLine("Test the album template methods:");
            try
            {
                // Create an album template
                myAlbumTemplate = user.CreateAlbumTemplate("TestAlbumTemplate");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Get the album templates for a user
            try
            {
                var myAlbumTemplatesList = user.GetAlbumTemplates();
                foreach (var x in myAlbumTemplatesList)
                {
                    Console.WriteLine(x.AlbumTemplateName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Change the settings for an album template
            try
            {
                myAlbumTemplate.Comments = false;
                myAlbumTemplate.AlbumTemplateName = "AlbumTemplateChangedName";
                myAlbumTemplate.ChangeSettings();
                Console.WriteLine("{0} - {1}", myAlbumTemplate.AlbumTemplateName, myAlbumTemplate.Comments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Delete an album template
            try
            {
                myAlbumTemplate.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Create an album template and save it to the site
            try
            {
                at = new AlbumTemplate();
                at.AlbumTemplateName = "AlbumTemplateNew";
                at.Public = false;
                at = user.CreateAlbumTemplate(at);
                Console.WriteLine(at.AlbumTemplateName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try
            {
                at.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();



            Console.WriteLine("Test the album functions: ");
            // Get all the albums from the site
            try
            {
                myAlbumList = user.GetAlbums(true);
                foreach (var x in myAlbumList)
                {
                    Console.WriteLine("{0}-{1}-{2}", x.Title, x.Category, (x.SubCategory == null) ? "None" : x.SubCategory.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Get info for an album from the site and return it in a new album (here I use the same object)
            try
            {
                myAlbumTitleOnly = myAlbumTitleOnly.GetInfo();
                Console.WriteLine(myAlbumTitleOnly.Protected);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Populate the current album with info from the site
            try
            {
                myAlbumInCategory.PopulateAlbumWithInfoFromSite();
                Console.WriteLine(myAlbumInCategory.Protected);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Change the settings for an album
            try
            {
                myAlbumTitleOnly.Protected = true;
                myAlbumTitleOnly.ChangeSettings();
                Console.WriteLine(myAlbumTitleOnly.Protected);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Comments
            try
            {
                myAlbumTitleOnly.AddComment("Comment for album");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Comments
            try
            {
                myAlbumTitleOnly.GetComments();
                if ((myAlbumTitleOnly.CommentsList != null) && (myAlbumTitleOnly.CommentsList.Count > 0))
                    Console.WriteLine(myAlbumTitleOnly.CommentsList[0].Text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Upload piture to the album from an URL
            try
            {
                myAlbumTitleOnly.UploadImageFromURL("http://www.socialseo.com/files/images/best-free.jpg");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Upload pictures to the album
            try
            {
                var up = myAlbumTitleOnly.CreateUploader();
                up.UploadCompleted += new EventHandler<UploadEventArgs>(img_UploadCompleted);
                up.UploadProgress += new EventHandler<UploadEventArgs>(img_UploadProgress);
                Console.Write("Give picture with whole path (eg: " + @"C:\Users\Username\Pictures\IMG_1234.jpg): ");
                String img1 = Console.ReadLine();
                Console.Write("Give picture with whole path (eg: " + @"C:\Users\Username\Pictures\IMG_1234.jpg): ");
                String img2 = Console.ReadLine();
                Console.Write("Give picture with whole path (eg: " + @"C:\Users\Username\Pictures\IMG_1234.jpg): ");
                String img3 = Console.ReadLine();
                up.UploadImage(img1);
                up.UploadImage(img2);
                up.UploadImage(img3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Comments
            try
            {

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Get all the images from the current album
            try
            {
                myImageList = myAlbumTitleOnly.GetImages(true);
                foreach (var x in myImageList)
                {
                    Console.WriteLine("{0} - {1}", x.FileName, x.Date);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Comments
            try
            {
                myImageList[0].AddComment("Comment on image");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Comments
            try
            {
                myImageList[0].GetComments();
                if (myImage2.Comments!=null)
                    Console.WriteLine(myImage2.Comments[0].Text);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Resort the album
            try
            {
                myAlbumTitleOnly.ReSort(By.FileName, Direction.DESC);
                foreach (var x in myImageList)
                {
                    Console.WriteLine("{0} - {1}", x.FileName, x.Aperture);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();



            Console.WriteLine("Test the image functions:");
            try
            {
                Image myImage = myImageList[0];
                myImage2 = myImageList[1];
                myImage3 = myImageList[2];
                // Change position
                myImage.ChangePosition(2);
                myImageList = myAlbumTitleOnly.GetImages(true);
                foreach (var x in myImageList)
                {
                    Console.WriteLine(x.FileName);
                }
                //// Crop the picture
                //myImage2.Crop(300, 300, 0, 10);
                // Change settings
                myImage.Caption = "Test image";
                myImage.ChangeSettings();
                Console.WriteLine("The new image caption is: " + myImage.Caption);
                // Get EXIF info from site
                myImage = myImage.GetEXIF();
                myImage2.PopulateImageWithEXIFFromSite();
                Console.WriteLine("EXIF info: " + myImage.Model);
                Console.WriteLine("EXIF info: " + myImage2.Model);
                // Get info from site
                myImage = myImage.GetInfo();
                myImage2.PopulateImageWithInfoFromSite();
                Console.WriteLine("Info: " + myImage.FileName);
                Console.WriteLine("Info: " + myImage2.FileName);
                // Get the URLs
                myImage = myImage.GetURLs();
                myImage2.PopulateImageWithURLsFromSite();
                Console.WriteLine(myImage2.OriginalURL);
                // Rotate an image
                myImage.Rotate(DegreesEnum.Left, false);
                //// Zoom the thumbnail
                //myImage3.ZoomThumbnail(100, 100, 1, 1);
                myImage.Delete();
                Console.WriteLine("The remaining pictures after delete: ");
                myImageList = myAlbumTitleOnly.GetImages(true);
                foreach (var x in myImageList)
                {
                    Console.WriteLine(x.FileName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();


            Console.WriteLine("Test the watermarking functions:");
            // Create a watermark
            try
            {
                myWatermark1 = myImage2.CreateWatermark("TestWatermark1");
                //myImage3.ApplyWatermark(myWatermark1);
                myWatermark2 = myImage3.CreateWatermark("TestWatermark2");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Display the existing watermarks
            try
            {
                var myWatermarksList = user.GetWatermarks(true);
                foreach (var x in myWatermarksList)
                {
                    Console.WriteLine(x.Name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            // Change settings
            try
            {
                myWatermark1.Name = "TestWatermarkNameChanged";
                myWatermark1.Dissolve = 50;
                // Get info
                myWatermark1 = myWatermark1.GetInfo();
                Console.WriteLine(myWatermark1.Name);
                myWatermark2.PopulateWatermarkWithInfoFromSite();
                Console.WriteLine(myWatermark2.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try
            {
                //myImage3.RemoveWatermark();
                myWatermark1.Delete();
                myWatermark2.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();



            // General use
            try
            {
                myAlbumTitleOnly.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try
            {
                myAlbumInCategory.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try 
            { 
                myAlbumInSubCategory.Delete(); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try
            {
                myCategory.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try
            {
                mySubCategory.Delete();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();

            try
            {
                // Disconnect from the site
                user.Logout();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
        }
    }
}
