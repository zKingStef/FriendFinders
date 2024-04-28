using DSharpPlus;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DarkBot.src.Database
{
    public class DBEngine
    {
        public string connectionString = "Host=bunjwqxejjdivx62llgi-postgresql.services.clever-cloud.com;" +
                                          "Username=ugaseupaiagerifnbppr;" +
                                          "Password=7BnA9hzCfoL46bt5W0n8Cf4jp80DKJ;" +
                                          "Database=bunjwqxejjdivx62llgi";

        public async Task<bool> StoreUserAsync(DiscordUserData user)
        {
            var userNum = await GetTotalUserAsync();

            if (userNum == -1)
            {
                throw new Exception();
            }
            else
            {
                userNum = userNum + 1;
            }

            try
            {
                using (var conn = new NpgsqlConnection(connectionString)) 
                {
                    await conn.OpenAsync();

                    string query = "INSERT INTO bmocfdpnmiqmcbuykudg.USERINFO (USER_ID, USERNAME, SERVERNAME, SERVER_ID)" +
                                  $"VALUES ('{userNum}', '{user.userName}', '{user.serverName}', '{user.serverID}')";

                    using (var cmd = new NpgsqlCommand(query, conn)) 
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                return true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private async Task<long> GetTotalUserAsync()
        {
            try 
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = "SELECT COUNT(*) FROM bmocfdpnmiqmcbuykudg.USERINFO;";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        var userCount = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt64(userCount);
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
    }
}
