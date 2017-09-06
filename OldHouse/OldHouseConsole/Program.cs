using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.Repository;
using Jtext103.Repository.Interface;
using Jtext103.MongoDBProvider;
using Jtext103.OldHouse.Business.Models;
using Jtext103.OldHouse.Business.Services;
using Excel=Microsoft.Office.Interop.Excel;
using Jtext103.EntityModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using Jtext103.BlogSystem;
using Jtext103.Identity.Service;
using Jtext103.Volunteer.VolunteerMessage;

namespace OldHouseConsole
{
    class Program
    {
        static HouseService houseService;
        static void Main(string[] args)
        {
            string remoteSrv = @"mongodb://115.156.252.5:27017";
            string localSrv = @"mongodb://127.0.0.1:27017";
            //setup db context
            var connection = localSrv;
            //todo ioc is really needed here
            MongoDBRepository<House> houseDb = new MongoDBRepository<House>(connection, "OldHouseDb", "House");
            MongoDBRepository<BlogPostEntity> CheckInDb = new MongoDBRepository<BlogPostEntity>(connection, "OldHouseDb", "CheckIn");
            MongoDBRepository<OldHouseUser> UserDb = new MongoDBRepository<OldHouseUser>(connection, "OldHouseDb", "OldHouseUser");
            MongoDBRepository<OldHouseUserProfile> ProfileDb = new MongoDBRepository<OldHouseUserProfile>(connection, "OldHouseDb", "OldHouseUserProfile");
            MongoDBRepository<Message> feedDb=new MongoDBRepository<Message>(connection, "OldHouseDb", "Feed");


            houseService = new HouseService(
                houseDb,
                new BlogPostService(CheckInDb, null),
                new EntityService<OldHouseUserProfile>(ProfileDb),
                null,   //the like rate is not used here
                new UserManager<OldHouseUser>(UserDb),
                new MessageService(feedDb));
            
            loadExcel(@"D:\OldHouse\code\OldHouse\TestDataSet\test2\data.xls", int.Parse(args[0]), int.Parse(args[1]));
            
            Console.ReadKey();

        }

        static void loadExcel(string file,int startrow, int endrow)
        {
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Visible = true;
            excel.Visible = false;

            Excel.Workbook theWorkbook;
            Excel.Worksheet worksheet;


            theWorkbook=excel.Workbooks.Open(file,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing);

            worksheet = theWorkbook.Worksheets[1];
            Excel.Range range;
            int succeeded = 0;
            int failed = 0;

            for(int row=startrow;row<=endrow;row++)
            {
                GeoPoint locPoint=new GeoPoint();
                string cellName;
                //check if there is already a house has the same code name if so skip it
                cellName = "A" + row.ToString();
                range= worksheet.get_Range(cellName, Missing.Value);
                string codeName=range.Value2.ToString();
                var tId = Guid.Empty;
                if (houseService.HouseExistByCodeName(codeName))
                {
                    tId = houseService.FindHouseByCodeName(codeName).Id;
                    Console.WriteLine("house exists");
                }
                //check if it has a name if not skip if
                cellName = "B" + row.ToString();
                range = worksheet.get_Range(cellName, Missing.Value);
                string houseName = range.Value2.ToString();
                if (string.IsNullOrEmpty(houseName))
                {
                    Console.WriteLine("no house name, skip");
                    failed++;
                    continue;
                }
                //chekck if the location is validte if not skip
                cellName = "C" + row.ToString();
                string location = worksheet.get_Range(cellName, Missing.Value).Value2.ToString();
                try
                {
                    locPoint = HouseService.GetGeoPoint(location);
                }
                catch
                {
                    failed++;
                    Console.WriteLine("invalid location, skip");
                    continue;
                }
                //all check noe build a house
                //user the code name to locate the photo folder
                var path = file.Substring(0, file.LastIndexOf(@"\") + 1) + @"photo\"+codeName;
                var photos =new List<string>();
                try
                {
                    photos = getPhotos(path,codeName).ToList<string>();
                }
                catch 
                {

                    Console.WriteLine("no photo");
                }
               
                DateTime year = DateTime.Parse("0001 06 06");
                try
                {
                    year = DateTime.Parse(worksheet.get_Range("I" + row.ToString(), Missing.Value).Value2.ToString() + " 06 01");
                }
                catch
                {
                    //no build year
                }
                House newHouse = new House
                {
                    CodeName = codeName,
                    Name = houseName,
                    Images = photos.Any() ? photos : null,
                    Cover = photos.Any() ? photos[0] : null,
                    Location = locPoint,
                    LocationString = worksheet.get_Range("D" + row.ToString(), Missing.Value).Value2.ToString(),
                    Abstarct = worksheet.get_Range("E" + row.ToString(), Missing.Value).Value2.ToString(),
                    Country = worksheet.get_Range("F" + row.ToString(), Missing.Value).Value2.ToString(),
                    Province = worksheet.get_Range("G" + row.ToString(), Missing.Value).Value2.ToString(),
                    City = worksheet.get_Range("H" + row.ToString(), Missing.Value).Value2.ToString(),
                    //BuiltYear = year,
                    //Condition = worksheet.get_Range("K" + row.ToString(), Missing.Value).Value2.ToString(),
                    //historica , photo value is not fixed yet
                    Description = worksheet.get_Range("P" + row.ToString(), Missing.Value).Value2.ToString(),
                };
                newHouse.AddExtraInformation("houseinfo-condition", worksheet.get_Range("K" + row.ToString(), Missing.Value).Value2.ToString());
                newHouse.AddExtraInformation("houseinfo-buildyear", year.Year);
                string tagString;
                try
                {
                    tagString = worksheet.get_Range("L" + row.ToString(), Missing.Value).Value2.ToString();
                    newHouse.Tags = tagString.Split(';').ToList<string>();
                }
                catch (Exception)
                {
                    newHouse.Tags = null;
                }
                
                var extra = worksheet.get_Range("Q" + row.ToString(), Missing.Value).Value2;
                if (extra != null)
                {
                    string extraStr=extra.ToString();
                    getExtra(extraStr, newHouse);
                }
                if (tId == Guid.Empty)
                {
                    houseService.InsertOne(newHouse);
                    succeeded++;
                    Console.WriteLine("house added: " + codeName);
                }
                else
                {
                    newHouse.Id = tId;
                    houseService.SaveOne(newHouse);
                    succeeded++;
                    Console.WriteLine("house updated: " + codeName);
                }
                
                
            }
            //clean up

            theWorkbook.Close(false, Type.Missing, Type.Missing);
            Marshal.ReleaseComObject(theWorkbook);

            excel.Quit();
            Marshal.FinalReleaseComObject(excel);
            Console.WriteLine("succeeded:"+succeeded);
            Console.WriteLine("failed:" + failed);

        }

        static IEnumerable<string> getPhotos(string path,string cName)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                yield return (
                    @"/content/Images/house/" + cName + @"/"+file.Substring(file.LastIndexOf(@"\") + 1, file.Length - file.LastIndexOf(@"\") - 1)
                    );
            }
        }

        static void getExtra(string extra, Entity ent)
        {
            if (string.IsNullOrEmpty(extra))
            {
                return ;
            }
            var exDict = new Dictionary<string, object>();
            var exs = extra.Split(';');
            foreach (var item in exs)
            {
                ent.ModifyExtraInformation(
                    "Basic-"+item.Substring(0,item.LastIndexOf(":")),
                    item.Substring(item.LastIndexOf(":")+1)
                    );
            }
            
 
        }

        

    }
}
