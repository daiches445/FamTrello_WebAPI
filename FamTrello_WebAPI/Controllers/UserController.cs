using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using FamTrello_WebAPI.Models;

namespace FamTrello_WebAPI.Controllers
{
    public class UserController : ApiController
    {
        readonly DBManager manager = new DBManager();
        


        [HttpGet]
        [Route("api/User/{username}")]
        
        public IHttpActionResult Get([FromUri]string username)
        {
            

            try
            {          
                User u = manager.GetUser(username);

                if (u != null)
                {
                    return Ok(u);
                }
                else
                    return Content(HttpStatusCode.NotFound, "user wasnt found");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
            
        }
        [HttpGet]
        [Route("api/User/sign_in")]

        public IHttpActionResult Signin([FromBody]User user2sign)
        {
           
            try
            {
                User u = manager.SignIn(user2sign);
                if (u != null)
                {
                    if (u.username == user2sign.username)
                        return Ok(u);
                    else
                        return Content(HttpStatusCode.NoContent, "Wrong password");
                }
                else
                    return Content(HttpStatusCode.NotFound, "");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }

        }

        [HttpGet]
        [Route("api/User/families/{username}")]
        public IHttpActionResult GetFamilies(string username)
        {

            try
            {
                List<Family> fam_lst = manager.GetFamilies(username).ToList();
                if (fam_lst.Count > 0)
                {
                    return Ok(fam_lst);
                }
                else
                    return Content(HttpStatusCode.NotFound, "Not Found.");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpGet]
        [Route("api/User/close_con")]
        public IHttpActionResult CloseCon()
        {
            try
            {
                manager.CloseCon();
                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest();
            }
        }


        [HttpPost]

        public IHttpActionResult Post([FromBody] User user2add)
        {
            try
            {
                User u  = manager.AddUser(user2add);
                if (u != null)
                {
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + u.username), u);
                }
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult Put([FromBody]User user2update)
        {
            try
            {
                User u = manager.UpdateUser(user2update);
                if (u != null)
                {
                    return Created(new Uri(Request.RequestUri.AbsoluteUri + u.username), u);
                }
                else
                    throw new Exception("Unavilable username");
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpDelete]
        [Route("api/User/{user2delete}")]
        public IHttpActionResult Delete([FromUri]string user2delete)
        {
            

            try
            {
                int res = manager.DeleteUser(user2delete);
                if (res > 0)
                {
                    return Content(HttpStatusCode.OK,"BYE " + user2delete + " I'll be Missing you!");
                }
                else
                {
                    return BadRequest("Unable to delete " + user2delete);
                }

            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}
