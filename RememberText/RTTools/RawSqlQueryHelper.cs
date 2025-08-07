using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RememberText.DAL.Context;
using RememberText.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Threading.Tasks;

namespace RememberText.RTTools
{
    public static class RawSqlQueryHelper
    {
        private static ILogger _logger;
        public static async Task<List<T>> RawSqlGetDinamicDataList<T>(RememberTextDbContext context, string query) where T : class, new()
        {
            var entities = new List<T>();

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                await context.Database.OpenConnectionAsync();
                using (var resultFromDb = await command.ExecuteReaderAsync())
                {
                    if(resultFromDb.HasRows)
                    {
                        while (await resultFromDb.ReadAsync()) // read data line by line
                        {
                            var newTemp = new T();
                            var newProperties = newTemp.GetType().GetProperties();

                            for (int y = 0; y < newProperties.Length; y++)
                            {
                                if(!string.IsNullOrEmpty(resultFromDb[newProperties[y].Name].ToString()))
                                {
                                    newTemp.GetType().GetProperty(newProperties[y].Name).SetValue(newTemp, resultFromDb[newProperties[y].Name]);
                                }
                            }

                            entities.Add(newTemp);
                        }
                    }
                }
            }

            return entities;
        }

        public static List<T> RawSqlReader<T>(RememberTextDbContext context, string query, Func<DbDataReader, T> map)
        {
            var entities = new List<T>();

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                context.Database.OpenConnectionAsync();

                using (var result = command.ExecuteReader())
                {
                    while(result.Read())
                    {
                        entities.Add(map(result));
                    }
                }
            }

            return entities;
        }

        public static async Task<List<string>> RawGetFieldStrs(RememberTextDbContext context, string query)
        {
            List<string> fieldStrs = new List<string>();
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                await context.Database.OpenConnectionAsync();
                using (var result = command.ExecuteReader())
                {
                    while (result.Read())
                    {
                        string fieldStr = result[0].ToString();
                        fieldStrs.Add(fieldStr);
                    }
                }
            }

            return fieldStrs;
        }

        public static async Task<ResponseFromRawQuery> RawNonQuery(RememberTextDbContext context, string query)
        {
            var response = new ResponseFromRawQuery();

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                try
                {
                    await context.Database.OpenConnectionAsync();
                    response.RespInt = await command.ExecuteNonQueryAsync();
                    response.RespStr = "The task was completed successfully!";
                }
                catch (Exception ex)
                {
                    LogError(_logger, ex.Message);
                    response.RespInt = 0;
                    //response.RespStr = ex.Message;
                    response.RespStr = "Changes Failed";
                }
            }

            return response;
        }

        public static async Task<ResponseFromRawQuery> RawUpdateTextContent(RememberTextDbContext context, string textContent, string tableName, int textId)
        {
            string query = @$"UPDATE {tableName} SET TextContent = @textContent, UpdateDateTime = @updateDateTime WHERE Id = @textId
                    UPDATE Topics SET UpdatedDateTime=@updateDateTime WHERE Id=(SELECT TopicId FROM {tableName} WHERE Id = @textId)";

            var response = new ResponseFromRawQuery();

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                SqlParameter TextId = new SqlParameter("@textId", SqlDbType.Int);
                TextId.Value = textId;
                SqlParameter UpdateDateTime = new SqlParameter("@updateDateTime", SqlDbType.DateTime);
                UpdateDateTime.Value = DateTime.Now;
                SqlParameter TextContent = new SqlParameter("@textContent", SqlDbType.NVarChar);
                TextContent.Value = textContent;
                command.Parameters.Add(TextId);
                command.Parameters.Add(UpdateDateTime);
                command.Parameters.Add(TextContent);

                try
                {
                    await context.Database.OpenConnectionAsync();
                    response.RespInt = await command.ExecuteNonQueryAsync();
                    response.RespStr = "The task was completed successfully!";
                }
                catch (Exception /*ex*/)
                {
                    response.RespInt = 0;
                    //response.RespStr = ex.Message;
                    response.RespStr = "Changes Failed";
                }
            }

            return response;
        }

        public static async Task<int> RawScalarSqlQuery(RememberTextDbContext context, string query)
        {
            int result = 0;
            using(var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = System.Data.CommandType.Text;

                await context.Database.OpenConnectionAsync();
                object scalResult = await command.ExecuteScalarAsync();

                if (int.TryParse(scalResult?.ToString(), out int scalInt))
                {
                    result = scalInt;
                }
            }

            return result;
        }

        #region Stored Procedure
        /// <summary>
        /// Get a List or a single data from a table by the Language Subtag and the Identifier 
        /// </summary>
        /// <typeparam name="T">Generics type for returning template class</typeparam>
        /// <param name="context">Database context</param>
        /// <param name="langSubtag">Language Subtag</param>
        /// <param name="queryId">Identifier</param>
        /// <param name="storedProcedure">Stored Procedure Name</param>
        /// <param name="template">Template class</param>
        /// <returns>List or single template class with data</returns>
        public static async Task<List<T>> SpDynamicGetTableData<T>(RememberTextDbContext context, string langSubtag, int queryId, string storedProcedure) where T : class, new()
        {
            var entities = new List<T>();

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcedure;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@languageSubtag", langSubtag));
                command.Parameters.Add(new SqlParameter("@queryId", queryId));

                await context.Database.OpenConnectionAsync();
                using (var resultFromDb = await command.ExecuteReaderAsync())
                {
                    while (await resultFromDb.ReadAsync()) // read data line by line
                    {
                        var newTemp = new T();
                        var newProperties = newTemp.GetType().GetProperties();

                        for (int y = 0; y < newProperties.Length; y++)
                        {
                            newTemp.GetType().GetProperty(newProperties[y].Name).SetValue(newTemp, resultFromDb[newProperties[y].Name]);
                        }

                        entities.Add(newTemp);
                    }
                }

                await context.Database.CloseConnectionAsync();
            }

            return entities;
        }

        public static async Task<int> SpDeleteRowById(RememberTextDbContext context, string tableName, int rowId)
        {
            int result = 0;
            using(var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "dbo.spDeleteRowById";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tableName", tableName));
                command.Parameters.Add(new SqlParameter("@rowId", rowId));

                await context.Database.OpenConnectionAsync();
                result = await command.ExecuteNonQueryAsync();
            }
            return result;
        }

        // Checking the existence of the row in the table in the database
        public static async Task<int?> SpIfRowExists(RememberTextDbContext context, string tableName, string rowName, string param)
        {
            int? result = null;
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "dbo.spEntityExists";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tableName", tableName));
                command.Parameters.Add(new SqlParameter("@rowName", rowName));
                command.Parameters.Add(new SqlParameter("@param", param));

                await context.Database.OpenConnectionAsync();
                object scalResult = await command.ExecuteScalarAsync();

                if (int.TryParse(scalResult?.ToString(), out int scalInt))
                {
                        result = scalInt;
                }

                await context.Database.CloseConnectionAsync();
            }

            return result;
        }

        // Checking the existence of the table in the database
        public static async Task<bool> SpIfTableExists(RememberTextDbContext context, string tableName)
        {
            bool result = false;
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "dbo.spTableExists";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@tableName", tableName));

                await context.Database.OpenConnectionAsync();
                object scalResult = await command.ExecuteScalarAsync();

                if (int.TryParse(scalResult?.ToString(), out int scalInt))
                {
                    if (scalInt > 0)
                        result = true;
                }

                await context.Database.CloseConnectionAsync();
            }

            return result;
        }

        #endregion Stored Procedure

        public static void LogInfo(ILogger logger, string logMessage)
        {
            logger.LogInformation(logMessage);
        }
        public static void LogError(ILogger logger, string logMessage)
        {
            logger.LogError(logMessage);
        }
    }
}
