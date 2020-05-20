using AzureBlobCrudDemo.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AzureBlobCrudDemo.Controllers
{
    public class AzureStorageController : Controller
    {
        public object CloudConfigurationManager { get; private set; }

        // GET: AzureStorage
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Index(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                // Get particular student info  
                TableManager TableManagerObj = new TableManager("person"); // 'person' is the name of the table  
                                                                           // pass query where RowKey eq value of id
                List<Student> SutdentListObj = TableManagerObj.RetrieveEntity<Student>("RowKey eq '" + id + "'");
                Student StudentObj = SutdentListObj.FirstOrDefault();
                return View(StudentObj);
            }
            return View(new Student());
        }

        [HttpPost]
        public ActionResult Index(string id, FormCollection formData)
        {
            Student StudentObj = new Student();
            StudentObj.Name = formData["Name"] == "" ? null : formData["Name"];
            StudentObj.Department = formData["Department"] == "" ? null : formData["Department"];
            StudentObj.Email = formData["Email"] == "" ? null : formData["Email"];

            // Insert  
            if (string.IsNullOrEmpty(id))
            {
                StudentObj.PartitionKey = "Student";
                StudentObj.RowKey = Guid.NewGuid().ToString();

                TableManager TableManagerObj = new TableManager("person");
                TableManagerObj.InsertEntity<Student>(StudentObj, true);
            }
            // Update  
            else
            {
                StudentObj.PartitionKey = "Student";
                StudentObj.RowKey = id;

                TableManager TableManagerObj = new TableManager("person");
                TableManagerObj.InsertEntity<Student>(StudentObj, false);
            }
            return RedirectToAction("Get");
        }

        public ActionResult Get()
        {
            TableManager TableManagerObj = new TableManager("person");
            List<Student> SutdentListObj = TableManagerObj.RetrieveEntity<Student>();
            return View(SutdentListObj);
        }

        public ActionResult Delete(string id)
        {
            TableManager TableManagerObj = new TableManager("person");
            List<Student> SutdentListObj = TableManagerObj.RetrieveEntity<Student>("RowKey eq '" + id + "'");
            Student StudentObj = SutdentListObj.FirstOrDefault();
            TableManagerObj.DeleteEntity<Student>(StudentObj);
            return RedirectToAction("Get");
        }

        public ActionResult QueueStorage()
        {
            // The below example is for Azure Queue Storage
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=demorgdiag378;AccountKey=zMM/LZWtqJhOYuVNLnnwE79q8Xj39D356tgdzGU7h5KQ6wcxpC85LtpXAX4ooHUZhhyn7/6fuLSg3/lwFlbs8Q==;EndpointSuffix=core.windows.net");
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("myqueue");
            queue.CreateIfNotExists();
            CloudQueueMessage message = new CloudQueueMessage("This message is from Azure Queue2");
            queue.AddMessage(message);
            //queue.Delete();
            return View();
        }
    }
}