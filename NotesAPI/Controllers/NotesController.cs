using Newtonsoft.Json.Linq;
using NotesAPI.Classes;
using NotesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace NotesAPI.Controllers
{
    public class NotesController : ApiController
    {
        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult Login()
        {
            if (Request.Headers.Authorization != null)
            {
                string body = Request.Content.ReadAsStringAsync().Result;
                string authenticationToken = Encoding.UTF8.GetString(Convert.FromBase64String(Request.Headers.Authorization.Parameter));
                string username = authenticationToken.Split(':')[0];
                string password = authenticationToken.Substring(username.Length + 1);
                string token = Utilities.GetSha256HashString(username + password + DateTime.Now.ToString());
                int userID = Database.VerifyUserCredentials(username, password);

                if (userID != 0)
                {
                    Database.InsertToken(userID, token);
                    LoginResponse loginResponse = new LoginResponse();
                    loginResponse.token = token;
                    return Ok(loginResponse);
                }
            }

            return BadRequest();
        }

        [NotesAuthorizationFilter]
        [HttpPost]
        [Route("api/logout")]
        public IHttpActionResult Logout(LogoutRequest logoutRequest)
        {
            try
            {
                Database.DeleteToken(logoutRequest.token);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        [NotesAuthorizationFilter]
        [HttpPost]
        [Route("api/newnote")]
        public IHttpActionResult NewNote([FromBody]NewNoteRequest newNoteRequest)
        {
            try
            {
                Database.AddNewNote(newNoteRequest.projectID, newNoteRequest.noteText, newNoteRequest.noteAttributes);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        [NotesAuthorizationFilter]
        [HttpPost]
        [Route("api/updatenote")]
        public IHttpActionResult UpdateNote([FromBody]UpdateNoteRequest updateNoteRequest)
        {
            try
            {
                Database.UpdateNote(updateNoteRequest.noteID, updateNoteRequest.noteText);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        [NotesAuthorizationFilter]
        [HttpPost]
        [Route("api/deletenote")]
        public IHttpActionResult DeleteNote([FromBody]DeleteNoteRequest deleteNoteRequest)
        {
            try
            {
                Database.DeleteNote(deleteNoteRequest.noteID);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }

        [NotesAuthorizationFilter]
        [HttpGet]
        [Route("api/getnotes")]
        public IHttpActionResult GetNotes(GetNotesRequest notesRequest)
        {
            List<Note> noteList = new List<Note>();
            try
            {
                noteList = Database.GetNotes(notesRequest.projectID, notesRequest.attributeIDs);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(noteList);
        }

        [NotesAuthorizationFilter]
        [HttpGet]
        [Route("api/getprojectnotecounts")]
        public IHttpActionResult GetProjectNoteCounts()
        {
            List<ProjectCount> projectNoteCountList = new List<ProjectCount>();
            try
            {
                projectNoteCountList = Database.GetProjectNoteCounts();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(projectNoteCountList);
        }

        [NotesAuthorizationFilter]
        [HttpGet]
        [Route("api/getattributenotecounts")]
        public IHttpActionResult GetAttributeNoteCounts()
        {
            List<AttributeCount> attributeNoteCountList = new List<AttributeCount>();
            try
            {
                attributeNoteCountList = Database.GetAttributeNoteCounts();
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok(attributeNoteCountList);
        }
    }
}
