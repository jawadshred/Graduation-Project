using System;
using System.Collections.Generic;
using System.Web.Mvc;
using turkcell_web_app.Models;
using turkcell_web_app.Models.Panels;
using turkcell_web_app.Models.Panels.Classes;
using turkcell_web_app.ViewModels;
using turkcell_web_app.Models.DB_Adapters;
using turkcell_web_app.Models.Session;
using turkcell_web_app.Models.FRD_Process;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;

namespace turkcell_web_app.Controllers
{

    [Authorize]
    public class RequestController : Controller
    {
        // GET: Request
        [HttpGet]
        public ActionResult New(String message)
        {
            ViewBag.Message = message;

            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }

            //check for saved FRD here

            byte[] file = DB_Functions.GetSaved();
            if (file != null)
            {
                if (String.IsNullOrEmpty(message))
                {
                    ViewBag.Message = "Previously saved FRD data loaded successfully. To erase, click on the RESET button";
                }

                Frd myfrd = G_Functions.DeserializeFromBytes<Frd>(file);

                myfrd.Type = Frd.TypesEnum.Saved;
                ViewBag.helpMessage = "•Panl0, Panel1 and Panel 7 are compulsory to fill, all other panels are optional. \n" +
                      "•Save To Draft Button will quickly save the currently inserted data for subsequent use.\n" +
                                  "•Reset Button will erase the currently inserted data and the most recent draft data, if any.";

                return View(SavedFrdHandling.ProcessInfo(myfrd));
            }

            //get list of target audience from database and store in variable a
            List<TargetAudience> a = DB_Functions.DefaultTargets();

            //get list of channels from database and store in variable b
            List<Channel> b = DB_Functions.DefaultChannels();


            //get list of departments with their respective employees inside from database and store in
            //variable c
            List<Distribution_Groups> c = DB_Functions.DefaultGroups();



            
            var senders = DB_Functions.DefaultSmsSenders();


            var viewModel = new Frd()
            {
                Type = Frd.TypesEnum.New,
                Upload_Token = DB_Functions.NewUploadToken(DB_Functions.UploadTokenType.New),
                Panel0 = new Panel_0()
                {
                    Owner = Functions.GetName()
                },
                Panel2 = new Panel_2()
                {
                    Targets = a
                },
                Panel3 = new Panel_3()
                {
                    Channels = b
                },
                Panel4 = new Panel_4()
                {
                    Senders = senders
                },
                Panel7 = new Panel_7()
                {
                    DistributionGroups = c
                }

            };
            ViewBag.helpMessage = "•Panl0, Panel1 and Panel 7 are compulsory to fill, all other panels are optional. \n" +
                                  "•Save To Draft Button will quickly save the currently inserted data for subsequent use.\n" +
                                  "•Reset Button will erase the currently inserted data and the most recent draft data, if any.";
            return View(viewModel);

        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult New(Frd submittedFrd)
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            switch (submittedFrd.SubmitType)
            {
                case Frd.ButtonsEnum.Reset://reset
                    DB_Functions.DeleteSavedDraft();
                    return RedirectToAction("New", new { message = "The FRD data has been reset successfully!" });

                case Frd.ButtonsEnum.Submit://submit
                    if (DB_Functions.SubmitNewFrd(submittedFrd))
                    {

                        SuccessData viewModel = new SuccessData()
                        {
                            Heading = "FRD has been submitted successfully",
                            PrimaryParagraph = "Your FRD has been submitted successfully!",
                            SecondaryParagraph = "Please wait for your manager's approval.",
                            LastParagraph = "The submitted FRD can be found in the 'Pending Requests' page.",
                            ButtonText = "GO TO PENDING REQUESTS",
                            ButtonLink = "Request/Pending"
                        };
                        return View("Success", viewModel);

                    }
                    else
                    {
                        submittedFrd.Type = Frd.TypesEnum.Saved;
                        return RedirectToAction("New", SavedFrdHandling.ProcessInfo(submittedFrd));
                    }

                case Frd.ButtonsEnum.Save://save
                    DB_Functions.SaveToDraft(G_Functions.ToByteArray(submittedFrd));
                    return RedirectToAction("New", new { message = "The FRD data has been saved successfully!" });
            }



            var a = FRD_Submit.Save(submittedFrd);
            return RedirectToAction("New");


        }






        [HttpGet]
        public ActionResult Active(string UserID = null)
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }

            if (UserID != null && !DB_Functions.CheckUserManager(UserID))
            {
                return new HttpNotFoundResult();
            }
            var a = DB_Functions.ActiveList(UserID);

            ViewBag.helpMessage = "This page contains a list of the FRDs that have been created by you and then approved. \n" +
                   "FRDs created by you and not approved yet can be found in the 'Pending Requests' page.\n";


            FrdsList viewModel = new FrdsList()
            {
                List = a,
                PageTitle = "Active Requets"
            };
            if (viewModel.List.Count > 0)
            {

                return View(viewModel);
            }
            else
            {
                NoFRD viewModel1 = new NoFRD()
                {
                    Heading = "You currently have no active FRDS available",
                    PrimaryParagraph = "To create and submit a new FRD, go to 'New Request' page",
                    SecondaryParagraph = "",
                    LastParagraph = "",
                    ButtonText = "GO TO NEW REQUEST",
                    ButtonLink = "Request/New",
                    Page = NoFRD.menuitem.Active
                };

                return View("noFRD", viewModel1);
            }
        }

        [HttpGet]
        public ActionResult Pending()
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            var a = DB_Functions.PendingList();

            ViewBag.helpMessage = "This page contains a list of the FRDs that have been created by you and have not been approved yet. \n" +
                "FRDs created by you and that have been approved can be found in the 'Active Requests' page.\n";


            FrdsList viewModel = new FrdsList()
            {
                List = a

            };
            if (viewModel.List.Count > 0)
            {
                return View(viewModel);
            }
            else
            {
                NoFRD viewModel1 = new NoFRD()
                {
                    Heading = "There are currently no pending FRDS available",
                    PrimaryParagraph = "FRDs appear as pending if you submitted an FRD and you are waiting for a manager's approval",
                    SecondaryParagraph = "To create and submit a new FRD, go to 'New Request' page",
                    LastParagraph = "",
                    ButtonText = "GO TO NEW REQUEST",
                    ButtonLink = "Request/New",
                    Page = NoFRD.menuitem.Pending
                };
                return View("noFRD", viewModel1);
            }
        }

        [HttpGet]
        public ActionResult Closed(string op = "all", string UserID = null)
        {
            string frdMessage = "";


            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            List<ActiveClosedFRDItem> list;
            int op_int = 0;
            if (UserID == null)
            {
                switch (op)
                {
                    case "all":
                        op_int = 0;
                        list = DB_Functions.ClosedList(op_int);
                        break;
                    case "me":
                        op_int = 1;
                        list = DB_Functions.ClosedList(op_int);
                        break;
                    case "other":
                        op_int = 2;
                        list = DB_Functions.ClosedList(op_int);
                        break;
                    default:
                        op_int = 0;
                        list = DB_Functions.ClosedList(op_int);
                        break;

                }
            }
            else
            {

                if (!DB_Functions.CheckUserManager(UserID))
                    return new HttpNotFoundResult();
                op_int = 1;
                list = DB_Functions.ClosedList(op_int, UserID);
            }


            var viewModel = new FrdsList()
            {
                List = list,
                PageTitle = "Closed Requests"
            };

            ViewBag.helpMessage = "This page contains a list of the FRDs that have been closed. \n" +
               "•All: All FRDs that have been closed and are related to you.\n•My FRDs: FRDs that you own and that have been closed. \n•Others' FRDs: FRDs that others own and that have been closed.";


            if (viewModel.List.Count > 0)
            {
                return View(viewModel);
            }
            else
            {
                switch (op)
                {
                    case "all":
                        frdMessage = "";

                        break;
                    case "me":
                        frdMessage = "owned by you";

                        break;
                    case "other":
                        frdMessage = "owned by others";
                        break;
                }


                NoFRD viewModel1 = new NoFRD()
                {
                    Heading = "There are currently no Closed FRDS " + frdMessage,
                    PrimaryParagraph = "FRDs " + frdMessage + " are listed here if they have been closed",
                    SecondaryParagraph = "Your active FRDs can be found in the 'Active Requests' page",
                    LastParagraph = "",
                    ButtonText = "GO TO ACTIVE REQUESTS",
                    ButtonLink = "Request/Active",
                    Page = op_int == 0 ? NoFRD.menuitem.ClosedAll : op_int == 1 ? NoFRD.menuitem.ClosedMe : NoFRD.menuitem.ClosedOthers
                };
                return View("noFRD", viewModel1);
            }

        }


        [HttpGet]
        public ActionResult ReceivedU(string UserID = null)
        {

            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            if (UserID != null && !DB_Functions.CheckUserManager(UserID))
                return new HttpNotFoundResult();
            FrdsList viewModel = new FrdsList
            {
                List = DB_Functions.ReceivedListAsUser(UserID)
            };


            ViewBag.helpMessage = "This page contains a list of the FRDs that you have received from other users. \n" +
         "The FRDs listed here are the ones you received because you were selected as a recepient of the FRD during its creation.";

            if (viewModel.List.Count > 0)
            {

                return View(viewModel);
            }
            else
            {
                NoFRD viewModel1 = new NoFRD()
                {
                    Heading = "There are currently no received FRDS available",
                    PrimaryParagraph = "Received FRDs are listed here if you were selected as a recepient of the FRD during its creation by an employee",
                    SecondaryParagraph = "",
                    LastParagraph = "",
                    ButtonText = "GO TO HOME PAGE",
                    ButtonLink = "Home/Index",
                    Page = NoFRD.menuitem.ReceivedU

                };
                return View("noFRD", viewModel1);
            }

        }


        [HttpGet]
        public ActionResult ReceivedM()
        {

            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            if (!Functions.IsManager())
            {
                return new HttpNotFoundResult();
            }

            FrdsList viewModel = new FrdsList { List = DB_Functions.ReceivedListAsManager() };

            ViewBag.helpMessage = "This page contains a list of the FRDs that you have received from other users. \n" +
       "The FRDs listed here are the ones you received because you are the manager of employees who submitted and FRD. \n" +
       "These FRDs are awaiting your approval or rejection in order to continue their process to the next phase";


            if (viewModel.List.Count > 0)
            {
                return View(viewModel);
            }
            else
            {
                NoFRD viewModel1 = new NoFRD()
                {
                    Heading = "There are currently no received FRDS available",
                    PrimaryParagraph = "Received FRDs are listed here if you are a manager of an employee who submitted an FRD",
                    SecondaryParagraph = "",
                    LastParagraph = "",
                    ButtonText = "GO TO HOME PAGE",
                    ButtonLink = "Home/Index",
                    Page = NoFRD.menuitem.ReceivedM

                };
                return View("noFRD", viewModel1);
            }

        }


        [HttpGet]
        public ActionResult ActiveFrd(string Id, string error = null)
        {
            ViewBag.message = error;
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }

            Frd File = DB_Functions.CheckActive(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(Id)));
            if (File == null)
            {
                //Error page
                return RedirectToAction("Active");
            }
            Frd viewModel = ActiveFrdProcessor.Process(File);

            ViewBag.helpMessage = "This FRD has been approved by your manager and is now awaiting the recepients' feedback and approval/rejection. \n" +
       "FRDs that you have created and are still awaiting approval can be found in the 'pending requests' page.";


            viewModel.Upload_Token = DB_Functions.NewUploadToken(DB_Functions.UploadTokenType.Active);
            return View(viewModel);

        }

        [HttpGet]
        public ActionResult PendingFrd(string Id)
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }

            Frd File = DB_Functions.CheckPending(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(Id)));
            if (File == null)
            {
                //Error page
                return RedirectToAction("Active");
            }
            Frd viewModel = ActiveFrdProcessor.Process(File);

            ViewBag.helpMessage = "This FRD you submitted is still awaiting approval by your manager. \n" +
                                  "Once approved, you will receive a notification and the FRD will be moved to the 'Active FRDs' page.";


            return View("View_NoEdit", viewModel);


        }


        [HttpGet]
        public ActionResult ReceivedFrd(string Id, string error = null)
        {
            ViewBag.message = error;
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }

            Frd File = DB_Functions.CheckReceived(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(Id)));
            if (File == null)
            {
                //Error page
                return RedirectToAction("Received");
            }
            Frd viewModel = ActiveFrdProcessor.Process(File);
            //viewModel.Type = Frd.TypesEnum.ReceivedManager; 



            if (viewModel.Type == Frd.TypesEnum.ReceivedManagerPending || viewModel.Type == Frd.TypesEnum.ReceivedManagerActive)
            {
                ViewBag.helpMessage = "This FRD is waiting for your approval. \n" +
                               "You can approve or reject this FRD.";

                return View("View_NoEdit", viewModel);//Manager Approval
            }
            viewModel.Upload_Token = DB_Functions.NewUploadToken(DB_Functions.UploadTokenType.Received);
            ViewBag.helpMessage = "This FRD is sent to you by the owner and is waiting for your feedback. \n" +
               "You can submit your feedback in the form of an approval, rejection, or by commenting and adding files and then submitting updates";
            return View(viewModel);


        }


        [HttpPost]
        public ActionResult ReceivedFrd(Frd document)
        {
            SuccessData viewModel;


            switch (document.SubmitType)
            {
                //reject, frd id is given back
                case Frd.ButtonsEnum.Reject:
                    if (document.Type == Frd.TypesEnum.ReceivedManagerPending) 
                    {

                        if (DB_Functions.ManagerReject(document.Panel0.Id, document.VersionNotes))
                        {
                            viewModel = new SuccessData()
                            {
                                Heading = "FRD has been refuted successfully",
                                PrimaryParagraph = "the FRD has now been closed!",
                                SecondaryParagraph = "",
                                LastParagraph = "The rejected FRD can be found in the 'Closed Requests' page.",
                                ButtonText = "GO TO CLOSED REQUESTS",
                                ButtonLink = "Request/Closed"
                            };
                            ViewBag.helpMessage = "No Information. \n";

                            return View("Success", viewModel);
                        }
           
                    }
                    else //if its others
                    {
                        if (DB_Functions.User_Reject(document.Panel0.Id)) 
                        {
                            viewModel = new SuccessData()
                            {
                                Heading = "FRD has been refuted successfully",
                                PrimaryParagraph = "",
                                SecondaryParagraph = "",
                                LastParagraph = "",
                                ButtonText = "GO TO HOME PAGE",
                                ButtonLink = "Home/Index"
                            };
                            ViewBag.helpMessage = "No Information. \n";

                            return View("Success", viewModel);
                        }
                    }



                    break;


                //approve, frd id is given back
                case Frd.ButtonsEnum.Approve:
                    if (document.Type == Frd.TypesEnum.ReceivedManagerPending)
                    {
                        if (DB_Functions.ManagerApprove(document.Panel0.Id))
                        {
                            viewModel = new SuccessData()
                            {
                                Heading = "FRD has been approved successfully",
                                PrimaryParagraph = "the active FRD submitted has been approved successfully!",
                                SecondaryParagraph = "The FRD has now been forwarded to the appropriate employees",
                                LastParagraph = "You can continue viewing this FRD on the 'RECEIVED REQUESTS' page",
                                ButtonText = "GO TO the RECEIVED REQUESTS page",
                                ButtonLink = "request/receivedM"
                            };
                            ViewBag.helpMessage = "No Information. \n";

                            return View("Success", viewModel);
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (DB_Functions.User_Approve(document.Panel0.Id))
                        {
                            viewModel = new SuccessData()
                            {
                                Heading = "FRD has been approved successfully",
                                PrimaryParagraph = "the active FRD submitted has been approved successfully!",
                                SecondaryParagraph = "The FRD has now been forwarded to the appropriate employees",
                                LastParagraph = "You can continue viewing this FRD on the 'RECEIVED REQUESTS' page",
                                ButtonText = "GO TO the RECEIVED REQUESTS page",
                                ButtonLink = "request/receivedU"
                            };
                            ViewBag.helpMessage = "No Information. \n";

                            return View("Success", viewModel);
                        }
                        else
                        {

                        }
                    }

                    break;


                //submit UPDATES, frd id & list of comments are given back
                case Frd.ButtonsEnum.Submit:
                    if (DB_Functions.SubmitReceiverUpdate(document))
                    {
                        viewModel = new SuccessData()
                        {
                            Heading = "FRD updates have been submitted successfully",
                            PrimaryParagraph = "The active FRD has been updated successfully!",
                            SecondaryParagraph = "The appropriate employees have now received your updated version",
                            LastParagraph = "The updated FRD can still be found in the 'Received Requests' page.",
                            ButtonText = "GO TO RECEIVED REQUESTS",
                            ButtonLink = "Request/ReceivedU"
                        };
                        ViewBag.helpMessage = "No Information. \n";

                        return View("Success", viewModel);
                    }
                    else
                    {

                        return RedirectToAction("ReceivedFrd", new { id = document.Panel0.Hashed_Id, error = "Changes have to be made in order to submit an updated version of the FRD" });

                    }


                default:
                    break;
            }

          
            return null;
        }

        [HttpPost]
        public ActionResult ActiveFrd(Frd submittedForm)
        {

            SuccessData viewModel;

            switch (submittedForm.SubmitType)
            {
                case Frd.ButtonsEnum.Submit:
                    if (DB_Functions.SubmitOwnerUpdate(submittedForm))
                    {
                        viewModel = new SuccessData()
                        {
                            Heading = "FRD updates have been submitted successfully",
                            PrimaryParagraph = "Your active FRD has been updated successfully!",
                            SecondaryParagraph = "The selected employees have now received your updated version",
                            LastParagraph = "The updated FRD can be found in the 'Active Requests' page.",
                            ButtonText = "GO TO ACTIVE REQUESTS",
                            ButtonLink = "Request/Active"
                        };
                        ViewBag.helpMessage = "No Information. \n";

                        return View("Success", viewModel);
                    }
                    else
                        return RedirectToAction("ActiveFrd", new { id = submittedForm.Panel0.Hashed_Id, error = "Changes have to be made in order to submit an updated version of the FRD" });
                case Frd.ButtonsEnum.Reject:
                    if (DB_Functions.OwnerClose(submittedForm.Panel0.Id))
                    {
                        viewModel = new SuccessData()
                        {
                            Heading = "FRD has been closed successfully",
                            PrimaryParagraph = "Your active FRD has been closed successfully!",
                            SecondaryParagraph = "",
                            LastParagraph = "The closed FRD can be found in the 'Closed Requests' page.",
                            ButtonText = "GO TO Closed REQUESTS",
                            ButtonLink = "Request/Closed"
                        };
                        ViewBag.helpMessage = "No Information. \n";

                        return View("Success", viewModel);
                    }
                    else
                    {

                    }


                    break;
            }

            ViewBag.helpMessage = "This FRD has been approved by your manager and is now awaiting the recepients' feedback and approval/rejection. \n" +
              "FRDs that you have created and are still awaiting approval can be found in the 'pending requests' page.";

            return View();
        }

        [System.Web.Mvc.Route("ViewFrd/{V}/{Id}")]
        [HttpGet]
        public ActionResult ViewFrd(string Id, int V)
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            Frd File = DB_Functions.CheckForFrd(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(Id)));
            if (File == null)
            {
                //Error page
                return new HttpNotFoundResult();
            }
            if (V != 0)
            {
                File.LatestVersion = V;
                File.Panel0.LatestVersion = V;
            }


            Frd s = ActiveFrdProcessor.Process(File, V);
            s.Type = Frd.TypesEnum.Closed;
            ViewBag.helpMessage = "This FRD can not be edited and can only be viewed from the archive of versions. \n";

            return View("View_NoEdit", s);
        }


        [HttpGet]
        public ActionResult ClosedFrd(string Id)
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }

            Frd File = DB_Functions.CheckClosed(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(Id)));
            if (File == null)
            {
                //Error page
                return RedirectToAction("Closed");
            }
            Frd s = ActiveFrdProcessor.Process(File);

            ViewBag.helpMessage = "This FRD can not be edited and can only be viewed from the archive of versions. \n";
            return View("View_NoEdit", s);


        }


        [HttpGet]
        public ActionResult Search(string input)
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            SearchResult viewModel = new SearchResult
            {
                Searched = input,
                List = DB_Functions.Search(input)
            };

            ViewBag.helpMessage = "Results of your search are displayed as a list of items. \n" +
                                  "inside each result item will be the relevant versions which match the search keyword";

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult SearchedFrd(string FrdId, int Version = 0)
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            Frd frd = DB_Functions.CheckForFrd(FrdId);
            if (frd == null)
                return new HttpNotFoundResult();

            if (Version == 0 || frd.LatestVersion == Version)
            {
                switch (frd.Type)
                {
                    case Frd.TypesEnum.Active:
                        return RedirectToAction("ActiveFrd", "Request", new { Id = G_Functions.GetHash(FrdId) });

                    case Frd.TypesEnum.Closed:
                        return RedirectToAction("ClosedFrd", "Request", new { Id = G_Functions.GetHash(FrdId) });
                        break;
                    case Frd.TypesEnum.Pending:
                        return RedirectToAction("PendingFrd", "Request", new { Id = G_Functions.GetHash(FrdId) });
                        break;
                    case Frd.TypesEnum.ReceivedManagerActive:


                    case Frd.TypesEnum.ReceivedManagerPending:


                    case Frd.TypesEnum.ReceivedUser:
                        return RedirectToAction("ReceivedFrd", "Request", new { Id = G_Functions.GetHash(FrdId) });


                        break;
                }
            }
            else
            {
                return RedirectToAction("ViewFrd", "Request", new { Id = G_Functions.GetHash(FrdId), V = Version });
            }
            return new HttpNotFoundResult();
        }




        [HttpGet]
        public ActionResult ManagerDashBoard()
        {
            if (Functions.NoSession())
            {
                return RedirectToAction("Login", "Authentication");
            }
            if (!Functions.IsManager())
            {
                return new HttpNotFoundResult();
            }

            var data = DB_Functions.GetManagerData();
            DashBoard viewModel = new DashBoard
            {
                List = data
            };
            ViewBag.helpMessage = "This page contains a list of the employees that you manage. \n" +
                                  "Under each employee, number of the various FRDs lists of that employee are shown";
            
            return View(viewModel);

        }

    }
}