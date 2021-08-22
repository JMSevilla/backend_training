using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web;
using backend_training.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace backend_training.Controllers
{
    [RoutePrefix("api/user")]
    public class tbusersController : ApiController
    {
        private mydatabaseEntities db = new mydatabaseEntities();
        registerClass reg = new registerClass();
        // GET: api/tbusers
        public IQueryable<tbuser> Gettbusers()
        {
            return db.tbusers;
        }

        // GET: api/tbusers/5
        [ResponseType(typeof(tbuser))]
        public IHttpActionResult Gettbuser(int id)
        {
            tbuser tbuser = db.tbusers.Find(id);
            if (tbuser == null)
            {
                return NotFound();
            }

            return Ok(tbuser);
        }

        // PUT: api/tbusers/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Puttbuser(int id, tbuser tbuser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tbuser.userID)
            {
                return BadRequest();
            }

            db.Entry(tbuser).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!tbuserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/tbusers
        [Route("developer-register"), HttpPost]
        public IHttpActionResult Posttbuser()
        {
            try
            {
                    APISecurity secure = new APISecurity();
                    var request = HttpContext.Current.Request;
                    reg.firstname = request.Form["firstname"];
                    reg.lastname = request.Form["lastname"];
                    reg.address = request.Form["primary"] + request.Form["secondary"];
                    reg.email = request.Form["email"];
                    reg.password = secure.Encrypt(request.Form["password"]);
                    reg.isverified = Convert.ToChar("1");
                    reg.isstatus = Convert.ToChar("1");
                    reg.istype = Convert.ToChar("1");
                    reg.createdat = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy/MM/dd h:m:s"));
                        if(string.IsNullOrEmpty(reg.firstname) || string.IsNullOrEmpty(reg.lastname)
                        || string.IsNullOrEmpty(reg.email) || string.IsNullOrEmpty(reg.password))
                {
                    return Ok("empty fields");
                }
                        else if(db.tbusers.Any(x => x.email == reg.email))
                {
                    return Ok("exist");
                }
                else
                {
                    tbuser users = new tbuser();
                    users.firstname = reg.firstname;
                    users.lastname = reg.lastname;
                    users.address = reg.address;
                    users.email = reg.email;
                    users.password = reg.password;
                    users.isverified = Convert.ToString(reg.isverified);
                    users.isstatus = Convert.ToString(reg.isstatus);
                    users.istype = Convert.ToString(reg.istype);
                    users.createdAt = reg.createdat;
                    users.istoken = "";
                    db.tbusers.Add(users);
                    db.SaveChanges();
                    return Ok("success register");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("code-entry"), HttpPost]
        public HttpResponseMessage CodeAdding(string vcode, string email)
        {
            try
            {
                using (db)
                {
                   if(db.verifiercodes.Any(x => x.email == email && x.isdone == "0"))
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, "exist verifier");
                    }
                    else
                    {
                        verifiercode ver = new verifiercode();
                        ver.vcode = vcode;
                        ver.email = email;
                        ver.isdone = "0";
                        ver.createdAt = Convert.ToDateTime(System.DateTime.Now.ToString("yyyy/MM/dd"));
                        db.verifiercodes.Add(ver);
                        db.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK, "success add verifier");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
        }
        [Route("send-verifier"), HttpPost]
        public async Task sendcode(string email, string vcode)
        {
            try
            {
                var htmlContent = "";
                var apikey = "SG.pku0MTEQSmu3RNponNoWeQ.uEEDzuT5AmTkIm0YfmoDWYRQENgMVLin3V02p4-c4tY";
                var client = new SendGridClient(apikey);
                var from = new EmailAddress("devopsbyte60@gmail.com", "Account Verification");
                var to = new EmailAddress(email.ToString(), "Modern Resolve");
                var subject = "Verification Code";
                var plainTextContent = "easy shit";
                htmlContent += "<center>";
                htmlContent += "<img src='https://scontent.fmnl16-1.fna.fbcdn.net/v/t1.6435-9/188048274_138145555030110_8243968313125335027_n.png?_nc_cat=101&ccb=1-3&_nc_sid=e3f864&_nc_eui2=AeHW5A9s4MdJBzpaCz5bOp5z2cG_1fHet8PZwb_V8d63ww_Zgo3ztDJ0ibFgYnoSWcs4p799I-66e3RHI7nb_N_h&_nc_ohc=jnVNHQUM8DwAX__g-qR&_nc_ht=scontent.fmnl16-1.fna&oh=bd61ab9ba79b3430e90952ca60c4969e&oe=612FE429' alt='No image' style='width: 50%; height: auto;' />'";
                htmlContent += "<h1>This is your verification code : " + vcode.ToString() + "</h1>";
                htmlContent += "</center>";
                var msg = MailHelper.CreateSingleEmail(
                    from,
                    to,
                    subject,
                    plainTextContent,
                    htmlContent
                    );
                await client.SendEmailAsync(msg);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [Route("check-input-code"), HttpPut]
        public IHttpActionResult check_code_verified(string vcode, string email)
        {
            try
            {
                using (db)
                {
                    var codechecker = db.verifiercodes.Any(x => x.vcode == vcode && x.email == email
                    && x.isdone == "0");
                    if (codechecker)
                    {
                        var objs = db.verifiercodes.Where(x => x.email == email).FirstOrDefault(); // palitan mo jm may mali dito -jm
                        if(objs != null)
                        {
                            objs.isdone = "1";
                            db.SaveChanges();
                            return Ok("verified");

                        }
                        else
                        {
                            return Ok("not found");
                        }
                    }
                    else
                    {
                        return Ok("not found");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // DELETE: api/tbusers/5
        [ResponseType(typeof(tbuser))]
        public IHttpActionResult Deletetbuser(int id)
        {
            tbuser tbuser = db.tbusers.Find(id);
            if (tbuser == null)
            {
                return NotFound();
            }

            db.tbusers.Remove(tbuser);
            db.SaveChanges();

            return Ok(tbuser);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool tbuserExists(int id)
        {
            return db.tbusers.Count(e => e.userID == id) > 0;
        }
    }
}