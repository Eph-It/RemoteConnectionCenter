using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMRDP.Models;
using System.Data;
using System.Data.SqlClient;

namespace CMRDP.Repository
{
    public class SQL : IDisposable
    {
        private RDPSettings _settings;
        private SqlConnection _connection;
        public SQL()
        {
            _settings = new RDPSettings();
            _connection = new SqlConnection($"Server={_settings.DBServer};Database={_settings.DBName};Trusted_Connection=True");
        }

        private void OpenConnection()
        {
            if(_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        private SqlCommand GetCommand(string commandText, Dictionary<string, object>Params = null)
        {
            var ret = new SqlCommand(commandText, _connection)
            {
                CommandType = CommandType.Text
            };
            foreach(var key in Params?.Keys)
            {
                ret.Parameters.AddWithValue(key, Params[key]);
            }
            return ret;
        }

        private List<Dictionary<string, object>> Execute(SqlCommand c)
        {
            List<Dictionary<string, object>> ret = new List<Dictionary<string, object>>();
            var read = c.ExecuteReader();
            while (read.Read())
            {
                var dic = new Dictionary<string, object>();
                for(int i = 0; i < read.FieldCount; i++)
                {
                    dic.Add(read.GetName(i), read.GetValue(i));
                }
                ret.Add(dic);
            }
            return ret;
        }

        public List<Dictionary<string, object>> Invoke(string query, Dictionary<string, object> Params = null)
        {
            var cmd = GetCommand(query, Params);
            OpenConnection();
            return Execute(cmd);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}