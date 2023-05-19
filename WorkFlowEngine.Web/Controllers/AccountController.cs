using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.Account;
using WorkFlowEngine.IRepository.Account;

namespace WorkFlowEngine.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILoginRepository _loginRepository;
        public IConfiguration Configuration { get; }
        public AccountController(ILoginRepository loginRepository, IConfiguration configuration)
        {
            _loginRepository = loginRepository;
            Configuration = configuration;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] Login Model)
        {
            try
            {
                if (!GenerateCaptcha.ValidateCaptchaCode(Model.strCaptcha, HttpContext))
                {
                    // TODO: Captcha validation failed, show error message      
                    //Model.Message = "Captcha validation failed.";
                    //return View(Model);
                    return Ok(new
                    {
                        status = 5,
                        msg = "Captcha validation failed."
                    });
                }
                else
                {
                    // TODO: captcha validation succeeded; execute the protected action  
                    //Model.VCHUSERNAME = AESEncrytDecry.DecryptStringAES(Model.VCHUSERNAME);
                    //Model.vchpassword = AESEncrytDecry.DecryptStringAES(Model.vchpassword);
                    if (Model.VCHUSERNAME == null)
                    {
                        //Model.Message = "Please enter user name.";
                        //return View(Model);
                        return Ok(new
                        {
                            status = 5,
                            msg = "Please enter user name."
                        });
                    }
                    else if (Model.vchpassword == null)
                    {
                        //Model.Message = "Please enter password.";
                        //return View(Model);
                        return Ok(new
                        {
                            status = 5,
                            msg = "Please enter password."
                        });
                    }
                    else
                    {
                        var Result = _loginRepository.GetLoginDetails(Model).Result.FirstOrDefault();
                        if (Result != null)
                        {
                            //var Aurl = Configuration.GetValue<string>("MySettings:AdminConsoleUrl");
                            HttpContext.Session.SetInt32("_UserId", Convert.ToInt32(Result.INTUSERID));
                            //HttpContext.Session.SetInt32("_RoleId", Convert.ToInt32(Result.ROLEID));
                            HttpContext.Session.SetInt32("_DeptId", Convert.ToInt32(Result.INTLEVELDETAILID));
                            HttpContext.Session.SetInt32("_DesignId", Convert.ToInt32(Result.intdesigid));
                            HttpContext.Session.SetInt32("_DistrictId", Convert.ToInt32(Result.INTLEVELDETAILID));
                            HttpContext.Session.SetString("_UserName", Result.vchfullname);
                            HttpContext.Session.SetString("_DesigName", Result.nvchdesigname);
                            //HttpContext.Session.SetString("_DeptName", Result.nvchlevelname);
                            ////HttpContext.Session.SetString("_Aurl", Aurl);
                            //HttpContext.Session.SetString("_EncryptUserName", CommonFunction.EncryptData(Result.VCHUSERNAME));
                            //HttpContext.Session.SetString("_MobileNo", Result.MOBILENO);

                            #region Required for Admin Console Authentication
                            HttpContext.Session.SetString("UserName", Result.VCHUSERNAME);
                            //  HttpContext.Session.SetString("Image", res.vchuserimage);
                            HttpContext.Session.SetString("UserId", Result.INTUSERID.ToString());
                            TempData["UserName"] = Result.VCHUSERNAME;
                            #endregion
                            //return RedirectToAction("Dashboard", "Home");
                            return Ok(new
                            {
                                status = 0,
                                url = "/Admin/Scheme"
                            });
                        }
                        else
                        {
                            //Model.Message = "The provided user name or password is incorrect.";
                            //return View(Model);
                            return Ok(new
                            {
                                status = 5,
                                msg = "The provided user name or password is incorrect."
                            });
                        }
                    }
                    // Reset the captcha if your app's workflow continues with the same view
                    //MvcCaptcha.ResetCaptcha("ExampleCaptcha");
                }
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, ex.Message);
                return View(Model);
                throw ex;

            }
        }
        [Route("get-captcha-image")]
        public FileStreamResult GetImage()
        {
            int width = 90;
            int height = 25;
            var captchaCode = GenerateCaptcha.Captcha();
            var result = GenerateCaptcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/GIF");
        }
        public IActionResult Logout()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }
}
