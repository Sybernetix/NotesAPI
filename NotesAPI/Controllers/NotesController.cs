using Newtonsoft.Json.Linq;
using NotesAPI.Classes;
using NotesAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NotesAPI.Controllers
{
    public class NotesController : ApiController
    {

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
