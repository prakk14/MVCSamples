using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Usermvc.Models;
using Usermvc.ViewModels;



namespace Usermvc.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users()
        {
            List<vwUSER> objUserlst = new List<vwUSER>();

            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "SELECT U.id, U.Username, U.EmailID, C.Country,S.StateName, U.CountryID, U.StateID,U.Active FROM [MT_USER] U join [MT_STATE] S on s.id=U.StateID and S.Active=1 join [MT_COUNTRY] C on C.id=U.CountryID and C.Active=1 order by U.Username";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    SqlDataReader objRe = objCmd.ExecuteReader();                    
                    while (objRe.Read())
                    {
                        vwUSER objUser = new vwUSER();
                        objUser.id = Convert.ToInt32(objRe["id"]);
                        objUser.Username = Convert.ToString(objRe["Username"]);
                        objUser.EmailID = Convert.ToString(objRe["EmailID"]);
                        objUser.CountryID = Convert.ToInt32(objRe["CountryID"]);
                        objUser.StateID = Convert.ToInt32(objRe["StateID"]);
                        objUser.CountryName = Convert.ToString(objRe["Country"]);
                        objUser.StateName = Convert.ToString(objRe["StateName"]);
                        objUser.Active = Convert.ToBoolean(objRe["Active"]);
                        objUserlst.Add(objUser);
                    }

                    objRe.Close();
                    objCmd.Dispose();
                    objConn.Close();
                }
            }
            catch (Exception ex)
            {
                vwUSER objUser = new vwUSER();
                objUserlst.Add(objUser);
                //code here
            }

            return View(objUserlst);
        }

        public ActionResult CreateUser()
        {
            vwUSER objUser = new vwUSER();
            objUser.lstCountry = new SelectList(new List<SelectListItem>(), "Value", "Text");
            objUser.lstState = new SelectList(new List<SelectListItem>(), "Value", "Text");
            return View(objUser);
        }

        [HttpPost]
        public ActionResult CreateUser(vwUSER objvwUSER)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "insert into [MT_USER](Username, CountryID, StateID, EmailID) VALUES(@Username,@CountryID,@StateID,@EmailID)";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    objCmd.Parameters.AddWithValue("@Username",objvwUSER.Username.Trim());
                    objCmd.Parameters.AddWithValue("@EmailID", objvwUSER.EmailID);
                    objCmd.Parameters.AddWithValue("@CountryID", objvwUSER.CountryName);
                    objCmd.Parameters.AddWithValue("@StateID", objvwUSER.StateName);

                    if (objCmd.ExecuteNonQuery() > 0)
                    {
                        objCmd.Dispose();
                        objConn.Close();

                        return RedirectToAction("Users");
                    }
                    else {
                        objCmd.Dispose();
                        objConn.Close();

                        return new EmptyResult();
                    }
                }
            }
            catch (Exception ex)
            {
                //code here

                return RedirectToAction("CreateUser");
            }
        }
        
        public ActionResult EditUser(int id)
        {
            vwUSER objUser = new vwUSER();

            objUser.lstCountry = new SelectList(new List<SelectListItem>(), "0", "--Select Country--");
            objUser.lstState = new SelectList(new List<SelectListItem>(), "0", "--Select State--");

            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "SELECT U.id, U.Username, U.EmailID, C.Country,S.StateName, U.CountryID, U.StateID,U.Active FROM [MT_USER] U join [MT_STATE] S on s.id=U.StateID and S.Active=1 join [MT_COUNTRY] C on C.id=U.CountryID and C.Active=1 WHERE U.id=@UserID order by U.Username";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    objCmd.Parameters.AddWithValue("@UserID", id);

                    SqlDataReader objRe = objCmd.ExecuteReader();
                    while (objRe.Read())
                    {
                        objUser.id = Convert.ToInt32(objRe["id"]);
                        objUser.Username = Convert.ToString(objRe["Username"]);
                        objUser.EmailID = Convert.ToString(objRe["EmailID"]);
                        objUser.CountryID = Convert.ToInt32(objRe["CountryID"]);
                        objUser.StateID = Convert.ToInt32(objRe["StateID"]);
                        objUser.CountryName = Convert.ToString(objRe["CountryID"]);
                        objUser.StateName = Convert.ToString(objRe["StateID"]);
                        objUser.Active = Convert.ToBoolean(objRe["Active"]);

                    }

                    objRe.Close();
                    objCmd.Dispose();
                    objConn.Close();
                }

                //Country ddl
                List<SelectListItem> lstCountryItem = new List<SelectListItem>();
                using (SqlConnection objConnC = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsersC = "SELECT id, Country FROM [MT_COUNTRY] WHERE Active=1 Order by [Country]";
                    SqlCommand objCmdC = new SqlCommand(qryUsersC, objConnC);

                    objConnC.Open();
                    SqlDataReader objReC = objCmdC.ExecuteReader();
                    while (objReC.Read())
                    {
                        SelectListItem objCountry = new SelectListItem();
                        objCountry.Text = Convert.ToString(objReC["Country"]);
                        objCountry.Value = Convert.ToString(objReC["id"]);
                        lstCountryItem.Add(objCountry);
                    }

                    objReC.Close();
                    objCmdC.Dispose();
                    objConnC.Close();
                }


                //State ddl
                List<SelectListItem> lstStateItem = new List<SelectListItem>();
                using (SqlConnection objConnS = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsersS = "SELECT id, StateName FROM  [MT_STATE] WHERE Active=1 AND [CountryID]=@CountryID Order by [StateName]";
                    SqlCommand objCmdS = new SqlCommand(qryUsersS, objConnS);

                    objConnS.Open();
                    objCmdS.Parameters.AddWithValue("@CountryID", objUser.CountryID);
                    SqlDataReader objReS = objCmdS.ExecuteReader();
                    while (objReS.Read())
                    {
                        SelectListItem objState = new SelectListItem();
                        objState.Text = Convert.ToString(objReS["StateName"]);
                        objState.Value = Convert.ToString(objReS["id"]);
                        lstStateItem.Add(objState);
                    }

                    objReS.Close();
                    objCmdS.Dispose();
                    objConnS.Close();
                }

                lstCountryItem.Add(new SelectListItem() { Value="0",Text="--Select Country--"});
                lstStateItem.Add(new SelectListItem() { Value = "0", Text = "--Select State--" });
                lstCountryItem.Find(m => m.Value == Convert.ToString(objUser.CountryID)).Selected = true;
                lstStateItem.Find(m => m.Value == Convert.ToString(objUser.StateID)).Selected = true;

                objUser.lstCountry = new SelectList(lstCountryItem, "Value", "Text");
                objUser.lstState = new SelectList(lstStateItem, "Value", "Text");
                return View(objUser);

            }
            catch (Exception ex)
            {
                //code here
                return View(objUser);
            }
        }

        [HttpPost]
        public ActionResult EditUser(int id,vwUSER objvwUSER)
        {

            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "UPDATE [MT_USER] SET Username=@Username, CountryID=@CountryID, StateID=@StateID, EmailID=@EmailID,Active=@Active WHERE id=@UserID";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    objCmd.Parameters.AddWithValue("@Username", objvwUSER.Username.Trim());
                    objCmd.Parameters.AddWithValue("@EmailID", objvwUSER.EmailID);
                    objCmd.Parameters.AddWithValue("@CountryID", objvwUSER.CountryName);
                    objCmd.Parameters.AddWithValue("@StateID", objvwUSER.StateName);
                    objCmd.Parameters.AddWithValue("@Active", objvwUSER.Active);
                    objCmd.Parameters.AddWithValue("@UserID", id);

                    if (objCmd.ExecuteNonQuery() > 0)
                    {
                        objCmd.Dispose();
                        objConn.Close();

                        return RedirectToAction("Users");
                    }
                    else
                    {
                        objCmd.Dispose();
                        objConn.Close();
                        return new EmptyResult();
                    }
                }
            }
            catch (Exception ex)
            {
                //code here
                return new EmptyResult();
            }
        }

        public ActionResult UserDetail(int id)
        {
            vwUSER objUser = new vwUSER();
            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "SELECT U.id, U.Username, U.EmailID, C.Country,S.StateName, U.CountryID, U.StateID,U.Active FROM [MT_USER] U join [MT_STATE] S on s.id=U.StateID and S.Active=1 join [MT_COUNTRY] C on C.id=U.CountryID and C.Active=1 WHERE U.id=@UserID order by U.Username";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    objCmd.Parameters.AddWithValue("@UserID", id);
                    SqlDataReader objRe = objCmd.ExecuteReader();
                    while (objRe.Read())
                    {   
                        objUser.id = Convert.ToInt32(objRe["id"]);
                        objUser.Username = Convert.ToString(objRe["Username"]);
                        objUser.EmailID = Convert.ToString(objRe["EmailID"]);
                        objUser.CountryID = Convert.ToInt32(objRe["CountryID"]);
                        objUser.StateID = Convert.ToInt32(objRe["StateID"]);
                        objUser.CountryName = Convert.ToString(objRe["Country"]);
                        objUser.StateName = Convert.ToString(objRe["StateName"]);
                        objUser.Active = Convert.ToBoolean(objRe["Active"]);
                    }

                    objRe.Close();
                    objCmd.Dispose();
                    objConn.Close();
                }
            }
            catch (Exception ex)
            {
                //code here
            }

            return View(objUser);
        }

        public ActionResult DeleteUser(int id)
        {
            vwUSER objUser = new vwUSER();
            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "SELECT U.id, U.Username, U.EmailID, C.Country,S.StateName, U.CountryID, U.StateID,U.Active FROM [MT_USER] U join [MT_STATE] S on s.id=U.StateID and S.Active=1 join [MT_COUNTRY] C on C.id=U.CountryID and C.Active=1 WHERE U.id=@UserID order by U.Username";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    objCmd.Parameters.AddWithValue("@UserID", id);
                    SqlDataReader objRe = objCmd.ExecuteReader();                    
                    while (objRe.Read())
                    {  
                        objUser.id = Convert.ToInt32(objRe["id"]);
                        objUser.Username = Convert.ToString(objRe["Username"]);
                        objUser.EmailID = Convert.ToString(objRe["EmailID"]);
                        objUser.CountryID = Convert.ToInt32(objRe["CountryID"]);
                        objUser.StateID = Convert.ToInt32(objRe["StateID"]);
                        objUser.CountryName = Convert.ToString(objRe["Country"]);
                        objUser.StateName = Convert.ToString(objRe["StateName"]);
                        objUser.Active = Convert.ToBoolean(objRe["Active"]);                        
                    }

                    objRe.Close();
                    objCmd.Dispose();
                    objConn.Close();
                }
            }
            catch (Exception ex)
            {   
                //code here
            }

            return View(objUser);
        }

        [HttpPost]
        public ActionResult DeleteUser(int id, FormCollection collec)
        {
            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "DELETE FROM MT_USER WHERE id=@UserID";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    objCmd.Parameters.AddWithValue("@UserID", id);

                    if (objCmd.ExecuteNonQuery() > 0)
                    {
                        objCmd.Dispose();
                        objConn.Close();

                        return RedirectToAction("Users");
                    }
                    else
                    {
                        objCmd.Dispose();
                        objConn.Close();
                        return new EmptyResult();
                    }
                }
            }
            catch (Exception ex)
            {
                //code here
                return new EmptyResult();
            }
        }


        public ActionResult Country()
        {
            List<SelectListItem> lstCountryItem = new List<SelectListItem>();
            //vwUSER objUser = new vwUSER();

            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "SELECT id, Country FROM [MT_COUNTRY] WHERE Active=1 Order by [Country]";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    SqlDataReader objRe = objCmd.ExecuteReader();
                    while (objRe.Read())
                    {
                        SelectListItem objCountry = new SelectListItem();
                        objCountry.Text = Convert.ToString(objRe["Country"]);
                        objCountry.Value= Convert.ToString(objRe["id"]);
                        lstCountryItem.Add(objCountry);
                    }

                    objRe.Close();
                    objCmd.Dispose();
                    objConn.Close();
                }
            }
            catch (Exception ex)
            {  
                //code here
            }

            return Json(new { lstCountryItem });

        }

        [HttpPost]
        public JsonResult State(string pCountry)
        {
            List<SelectListItem> lstStateItem = new List<SelectListItem>();
            //vwUSER objUser = new vwUSER();

            try
            {
                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["UsermvcContext"].ToString()))
                {
                    string qryUsers = "SELECT id, StateName FROM  [MT_STATE] WHERE Active=1 AND [CountryID]=@CountryID Order by [StateName]";
                    SqlCommand objCmd = new SqlCommand(qryUsers, objConn);

                    objConn.Open();
                    objCmd.Parameters.AddWithValue("@CountryID", pCountry);
                    SqlDataReader objRe = objCmd.ExecuteReader();
                    while (objRe.Read())
                    {
                        SelectListItem objState = new SelectListItem();
                        objState.Text = Convert.ToString(objRe["StateName"]);
                        objState.Value = Convert.ToString(objRe["id"]);
                        lstStateItem.Add(objState);
                    }

                    objRe.Close();
                    objCmd.Dispose();
                    objConn.Close();
                }
            }
            catch (Exception ex)
            {
                //code here
            }
                        
            return Json(new { lstStateItem });
        }


    }
}