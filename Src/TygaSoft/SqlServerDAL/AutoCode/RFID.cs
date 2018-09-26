﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using TygaSoft.IDAL;
using TygaSoft.Model;
using TygaSoft.DBUtility;

namespace TygaSoft.SqlServerDAL
{
    public partial class RFID : IRFID
    {
        #region IRFID Member

        public int Insert(RFIDInfo model)
        {
            StringBuilder sb = new StringBuilder(300);
            sb.Append(@"insert into RFID (TID,EPC,LastUpdatedDate)
			            values
						(@TID,@EPC,@LastUpdatedDate)
			            ");

            SqlParameter[] parms = {
                                       new SqlParameter("@TID",SqlDbType.VarChar,50),
new SqlParameter("@EPC",SqlDbType.VarChar,50),
new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.TID;
            parms[1].Value = model.EPC;
            parms[2].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), parms);
        }

        public int Update(RFIDInfo model)
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append(@"update RFID set LastUpdatedDate = @LastUpdatedDate 
			            where TID = @TID and EPC = @EPC
					    ");

            SqlParameter[] parms = {
                                     new SqlParameter("@TID",SqlDbType.VarChar,50),
new SqlParameter("@EPC",SqlDbType.VarChar,50),
new SqlParameter("@LastUpdatedDate",SqlDbType.DateTime)
                                   };
            parms[0].Value = model.TID;
            parms[1].Value = model.EPC;
            parms[2].Value = model.LastUpdatedDate;

            return SqlHelper.ExecuteNonQuery(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), parms);
        }

        public int Delete(string tID, string ePC)
        {
            StringBuilder sb = new StringBuilder(250);
            sb.Append("delete from RFID where TID = @TID and EPC = @EPC ");
            SqlParameter[] parms = {
                                     new SqlParameter("@TID",SqlDbType.VarChar,50),
new SqlParameter("@EPC",SqlDbType.VarChar,50)
                                   };
            parms[0].Value = tID;
            parms[1].Value = ePC;

            return SqlHelper.ExecuteNonQuery(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), parms);
        }

        public bool DeleteBatch(IList<object> list)
        {
            StringBuilder sb = new StringBuilder(500);
            ParamsHelper parms = new ParamsHelper();
            int n = 0;
            foreach (string item in list)
            {
                n++;
                sb.Append(@"delete from RFID where TID = @TID" + n + " ;");
                SqlParameter parm = new SqlParameter("@TID" + n + "", SqlDbType.UniqueIdentifier);
                parm.Value = Guid.Parse(item);
                parms.Add(parm);
            }

            return SqlHelper.ExecuteNonQuery(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), parms != null ? parms.ToArray() : null) > 0;
        }

        public RFIDInfo GetModel(string tID, string ePC)
        {
            RFIDInfo model = null;

            StringBuilder sb = new StringBuilder(300);
            sb.Append(@"select top 1 TID,EPC,LastUpdatedDate 
			            from RFID
						where TID = @TID and EPC = @EPC ");
            SqlParameter[] parms = {
                                     new SqlParameter("@TID",SqlDbType.VarChar,50),
new SqlParameter("@EPC",SqlDbType.VarChar,50)
                                   };
            parms[0].Value = tID;
            parms[1].Value = ePC;

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), parms))
            {
                if (reader != null)
                {
                    if (reader.Read())
                    {
                        model = new RFIDInfo();
                        model.TID = reader.GetString(0);
                        model.EPC = reader.GetString(1);
                        model.LastUpdatedDate = reader.GetDateTime(2);
                    }
                }
            }

            return model;
        }

        public IList<RFIDInfo> GetList(int pageIndex, int pageSize, out int totalRecords, string sqlWhere, params SqlParameter[] cmdParms)
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append(@"select count(*) from RFID ");
            if (!string.IsNullOrEmpty(sqlWhere)) sb.AppendFormat(" where 1=1 {0} ", sqlWhere);
            totalRecords = (int)SqlHelper.ExecuteScalar(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), cmdParms);

            if (totalRecords == 0) return new List<RFIDInfo>();

            sb.Clear();
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;

            sb.Append(@"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,
			          TID,EPC,LastUpdatedDate
					  from RFID ");
            if (!string.IsNullOrEmpty(sqlWhere)) sb.AppendFormat(" where 1=1 {0} ", sqlWhere);
            sb.AppendFormat(@")as objTable where RowNumber between {0} and {1} ", startIndex, endIndex);

            IList<RFIDInfo> list = new List<RFIDInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RFIDInfo model = new RFIDInfo();
                        model.TID = reader.GetString(1);
                        model.EPC = reader.GetString(2);
                        model.LastUpdatedDate = reader.GetDateTime(3);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public IList<RFIDInfo> GetList(int pageIndex, int pageSize, string sqlWhere, params SqlParameter[] cmdParms)
        {
            StringBuilder sb = new StringBuilder(500);
            int startIndex = (pageIndex - 1) * pageSize + 1;
            int endIndex = pageIndex * pageSize;

            sb.Append(@"select * from(select row_number() over(order by LastUpdatedDate desc) as RowNumber,
			           TID,EPC,LastUpdatedDate
					   from RFID ");
            if (!string.IsNullOrEmpty(sqlWhere)) sb.AppendFormat(" where 1=1 {0} ", sqlWhere);
            sb.AppendFormat(@")as objTable where RowNumber between {0} and {1} ", startIndex, endIndex);

            IList<RFIDInfo> list = new List<RFIDInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RFIDInfo model = new RFIDInfo();
                        model.TID = reader.GetString(1);
                        model.EPC = reader.GetString(2);
                        model.LastUpdatedDate = reader.GetDateTime(3);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public IList<RFIDInfo> GetList(string sqlWhere, params SqlParameter[] cmdParms)
        {
            StringBuilder sb = new StringBuilder(500);
            sb.Append(@"select TID,EPC,LastUpdatedDate
                        from RFID ");
            if (!string.IsNullOrEmpty(sqlWhere)) sb.AppendFormat(" where 1=1 {0} ", sqlWhere);
            sb.Append("order by LastUpdatedDate desc ");

            IList<RFIDInfo> list = new List<RFIDInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString(), cmdParms))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RFIDInfo model = new RFIDInfo();
                        model.TID = reader.GetString(0);
                        model.EPC = reader.GetString(1);
                        model.LastUpdatedDate = reader.GetDateTime(2);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        public IList<RFIDInfo> GetList()
        {
            StringBuilder sb = new StringBuilder(300);
            sb.Append(@"select TID,EPC,LastUpdatedDate 
			            from RFID
					    order by LastUpdatedDate desc ");

            IList<RFIDInfo> list = new List<RFIDInfo>();

            using (SqlDataReader reader = SqlHelper.ExecuteReader(SqlHelper.WmsDbConnString, CommandType.Text, sb.ToString()))
            {
                if (reader != null && reader.HasRows)
                {
                    while (reader.Read())
                    {
                        RFIDInfo model = new RFIDInfo();
                        model.TID = reader.GetString(0);
                        model.EPC = reader.GetString(1);
                        model.LastUpdatedDate = reader.GetDateTime(2);

                        list.Add(model);
                    }
                }
            }

            return list;
        }

        #endregion
    }
}
