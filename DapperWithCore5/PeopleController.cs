using Microsoft.AspNetCore.Mvc;
using System;
using Dapper;
using System.Threading.Tasks;
using System.Collections;
using System.Data;
using Dapper.Model.Models;

namespace DapperWithCore5
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private DapperContext _dapperContext;  
        public PeopleController(DapperContext dapperContext)
        {
            _dapperContext=dapperContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            IEnumerable peopleLst;
            try
            {
               
                var query = "SELECT * FROM Person.Person";
                using (var connection = _dapperContext.CreateConnection())
                {
                    peopleLst = await connection.QueryAsync<People>(query);
                }
                return Ok(peopleLst);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Get-Person/{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                People person;
                var query = "SELECT * FROM Person.Person where BusinessEntityID= @id";
                using (var connection = _dapperContext.CreateConnection())
                {
                     person = await connection.QueryFirstOrDefaultAsync<People>(query, new { id });
                }

                return Ok(person);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Get-EmailAddress")]
        public async Task<IActionResult> GetEmailAddress()
        {
            IEnumerable peopleLst;
            IEnumerable Emails;

            try
            {

                var query = @"SELECT top 10* FROM Person.Person; SELECT * FROM Person.EmailAddress; ";
                using (var connection = _dapperContext.CreateConnection())
                {

                    using (var lists = await connection.QueryMultipleAsync(query))
                    {
                        peopleLst = lists.Read<People>();
                        Emails = lists.Read<Email_Address>();
                        
                    }
                }
                return Ok(Emails);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("update-EmailAddress")]
        public async Task<IActionResult> UpdateEmailAddress([FromBody] Email_Address email)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("businessEntityID", email.BusinessEntityID, DbType.Int32);
                parameters.Add("ModifiedDate", email.ModifiedDate, DbType.DateTime);
                parameters.Add("EmailAddress", email.EmailAddress, DbType.String);
                parameters.Add("EmailAddressID", email.EmailAddressID, DbType.Int32);
                var query = @"update Person.EmailAddress set businessEntityID=@businessEntityID , ModifiedDate=@ModifiedDate, EmailAddress=@EmailAddress where EmailAddressID=@EmailAddressID";
                using (var connection = _dapperContext.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("add-person")]
        public async Task<IActionResult> AddPerson([FromBody] People person)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FirstName", person.FirstName, DbType.String);
                parameters.Add("BusinessEntityID", person.BusinessEntityID, DbType.Int32);
                parameters.Add("LastName", person.LastName, DbType.String);
                parameters.Add("MiddleName", person.MiddleName, DbType.String);
                parameters.Add("PersonType", person.PersonType, DbType.String);
                parameters.Add("Title", person.Title, DbType.String);
                parameters.Add("Suffix", person.Suffix, DbType.String);
                parameters.Add("ModifiedDate", person.ModifiedDate, DbType.DateTime);    
                var query = @"insert into Person.Person (BusinessEntityID,FirstName,MiddleName,LastName,PersonType,Title,Suffix,ModifiedDate)
                             values(@BusinessEntityID,@FirstName,@MiddleName ,@LastName,@PersonType,@Title,@Suffix,@ModifiedDate)";
                using (var connection = _dapperContext.CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
