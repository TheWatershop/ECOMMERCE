using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
class MainClass
{
private static string License = "j9qQymxhiFXwVRYKQCIK6w==";
static int Main(string[] args)
{
    //try
    //{
        //System.Console.WriteLine("Testing");
        DateTime DT;// = Convert.ToDateTime(args[2]);
        //if (args[0] != "DELPIC" && args[0] != "PIC")
        //    DT = Convert.ToDateTime(args[2]);
        try
        {
            if (License != EncryptString(System.Environment.MachineName))
                return 0;
            else
            {
                if (args[0] == "SEL")
                {                    
                    if (args[1] == "")
                    {
                        //int Success = 0;
                        Functions.FromSql(args[0], "CAT", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "ROLE", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "CUST", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "ITEM", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "SH", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        Functions.FromSql(args[0], "SHL", Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        //return 0;
                    }
                    else
                        Functions.FromSql(args[0], args[1], Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                }
                else if (args[0] == "DELPIC")
                {
                    System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(args[1]);
                    foreach (System.IO.FileInfo file in dInfo.GetFiles())
                        file.Delete();
                    //return 0;
                }
                else if (args[0] == "PIC")
                {
                    System.IO.DirectoryInfo dInfo = new System.IO.DirectoryInfo(args[1]);
                    bool FirstRun = true;
                    if (dInfo.FullName.Contains("ITEM"))
                        FirstRun = false;

                    foreach (System.IO.FileInfo file in dInfo.GetFiles())
                    {
                        Functions.CreatePic(file.FullName.Contains("CAT") ? "CAT" : "ITEM", file.Name.Replace(".BMP", ""), file.FullName, FirstRun);
                        if (FirstRun)
                            FirstRun = false;
                    } //return 0;
                }
                else if (args[0] == "QTY")
                {
                    Functions.SetQty(args[4]);
                    //return 0;
                }
                else
                {
                    DT = Convert.ToDateTime(args[2]);
                    List<string> types = new List<string>();
                    types.Add("CAT"); types.Add("ROLE"); types.Add("CUST"); types.Add("ITEM"); types.Add("PRICE");
                    types.Add("SADDR"); types.Add("SH"); types.Add("SHL"); types.Add("COMP");
                    //qty
                    if (File.Exists(args[4] + "\\" + "QTY" + ".txt"))
                    {
                        Functions.SetQty(args[4] + "\\QTY.txt");
                        File.Delete(args[4] + "\\" + "QTY" + ".txt");
                    }
                    else
                    {
                        //for each type
                        foreach (string str in types)
                        {
                            if (File.Exists(args[4] + "\\" + str + ".txt"))
                            {
                                Functions.ToSql(args[0], str, Convert.ToDateTime(args[2]).ToUniversalTime(), "", new List<SqlParameter>(), args[4] + "\\" + str + ".txt");
                                //if (str != "COMP")
                                File.Delete(args[4] + "\\" + str + ".txt");
                            }
                        }

                        //sel
                        types.Remove("PRICE");
                        types.Remove("SADDR");
                        types.Remove("COMP");
                        foreach (string str in types)
                            Functions.FromSql(args[0], str, Convert.ToDateTime(args[2]).ToUniversalTime(), args[3], 0);
                        //return Functions.ToSql(args[0], args[1], Convert.ToDateTime(args[2]).ToUniversalTime(), "", new List<SqlParameter>(), args[4]);
                    }
                }
                return 0;
            }
        }
        catch (Exception ex)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
            file.WriteLine("Main" + ex.ToString());
            file.Close();
            return 0;
        }
    //}
}

private static string EncryptString(string ClearText)
{
byte[] clearTextBytes = Encoding.UTF8.GetBytes(ClearText);
System.Security.Cryptography.SymmetricAlgorithm rijn = SymmetricAlgorithm.Create();
MemoryStream ms = new MemoryStream();
byte[] rgbIV = Encoding.ASCII.GetBytes("ryojvlzmdalyglrj");
byte[] key = Encoding.ASCII.GetBytes("hcxilkqbbhczfeultgbskdmaunivmfuo");
CryptoStream cs = new CryptoStream(ms, rijn.CreateEncryptor(key, rgbIV),
CryptoStreamMode.Write);
cs.Write(clearTextBytes, 0, clearTextBytes.Length);
cs.Close();
return Convert.ToBase64String(ms.ToArray());
}
}
#region //Import Class and Functions
public class Functions
{
private static string NAVAuth = "Server = ECOMMERCE; Database = watershop; User Id = SA;Password = 99226400!;";
private static string NCAuth = "Server = ECOMMERCE; Database = NOP; User Id = SA;Password = 99226400!;";
public static void ToSql(string Direction, string RecType, DateTime DT, string CompName, List<SqlParameter> sl, string BT)
 {
     string line = "";
    try
    {
        //int outp = 1;
        using (SqlConnection con = new SqlConnection(Direction == "SET" ? NAVAuth : NCAuth))
        using (SqlCommand cmd = new SqlCommand(Direction == "SET" ? "dbo.CESP_NAVSET" : "dbo.CESP_" + RecType + Direction, con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            if (Direction == "PROC")
            {
                //System.IO.StreamWriter file;// = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true); //for errors
                //file.WriteLine("Check1 before sr");
                //file.Close();
                string mapupd = "";
                string Index = "";
                using (StreamReader sr = new StreamReader(BT))
                {                    
                    //file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                    //file.WriteLine("Check2 before split");
                    //file.Close();
                    string[] str = sr.ReadToEnd().Split('*');  
                    foreach(string st in str)
                    {
                        try
                        {
                            line = st;
                            cmd.Parameters.Clear();
                            sl.Clear();
                            string[] values = st.Split(',');
                            //file.Close();
                            Index = values[values.Length - 1];
                            GetParams(ref sl, RecType, Direction, st, DT);
                            //
                            foreach (SqlParameter sp in sl)
                                cmd.Parameters.Add(sp);
                            SqlParameter output = new SqlParameter("@RetNo", SqlDbType.Int);
                            //file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                            //file.WriteLine("Check6 after all, before mapupd" + Index + "/" + output.Value);
                            //file.Close();
                            output.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(output);
                            cmd.ExecuteNonQuery();
                            //file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                            //file.WriteLine("chk 7 After Exct:" + output.Value);
                            //file.Close();
                            if (values[0] == "0")
                                mapupd += Index + "," + output.Value + ",";
                            //if (mapupdcomma == "")
                            //    mapupdcomma = ",";
                        }
                        catch(Exception ex)
                        {
                            System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                            file.WriteLine("Error: Possible SQL: ToSql: " + DT + " Rec/Dir/BT:" + RecType + "/" + Direction + "/" + BT + ":Line:" + line);
                            file.WriteLine(ex.ToString());
                            file.Close();
                            //return 0;
                        }
                    }
                } // >>
                //file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                //file.WriteLine("chk 8 before final map" + mapupd);
                //file.Close();
                using (SqlConnection con2 = new SqlConnection(NAVAuth))
                using (SqlCommand cmd2 = new SqlCommand("dbo.CESP_MAPUPD", con2))
                {
                    cmd2.CommandType = CommandType.StoredProcedure;
                    con2.Open();
                    SqlParameter sp = new SqlParameter("@csv", "NVarChar");
                    sp.Value = mapupd;
                    cmd2.Parameters.Add(sp);
                    
                    cmd2.ExecuteNonQuery();
                } // <<
                //file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                //file.WriteLine("chk 9 after mapupd" + mapupd);
                //file.Close();
            }
            else
            {
                foreach (SqlParameter sp in sl)
                    cmd.Parameters.Add(sp);
                cmd.ExecuteNonQuery();
                if (RecType == "SH" && Direction == "SET")
                    foreach (SqlParameter sp in sl)
                        if (sp.ParameterName == "@NCNo")
                            FromSql("SEL", "SHL", DT, CompName, (int)sp.Value);
            }
            //return 0;
        }        
    }
    catch (Exception ex)
    {
        System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt",true);
        file.WriteLine("Error: Possibly File Related: ToSql:  "+ DT + " Rec/Dir/BT:" + RecType +"/"+ Direction +"/"+ BT + ":Line:" + line);
        file.WriteLine(ex.ToString());
        file.Close();
        //return 0;
    }
}
public static void FromSql(string Direction, string RecType, DateTime DT, string CompName, int DocNo)
 {
//int Success = 1;
if (RecType != "ROLE" && Direction != "Sel")
{
    using (SqlConnection con = new SqlConnection(NCAuth))
    using (SqlCommand cmd = new SqlCommand("dbo.CESP_"+RecType+"SEL", con))
    {
        cmd.CommandType = CommandType.StoredProcedure;
        con.Open();
        if (RecType == "SHL" || RecType == "SADDR")
            cmd.Parameters.Add(new SqlParameter("@DocNo", SqlDbType.Int)).Value = DocNo;
        else
            cmd.Parameters.Add(new SqlParameter("@LastDate", SqlDbType.DateTime)).Value = DT;
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                try
                {

                    List<SqlParameter> sl = new List<SqlParameter>();
                    SqlParameter sp;
                    #region //params;
                    switch (RecType)
                    {
                        case "CAT":
                            {
                                sp = new SqlParameter("@NCNo", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@RecType", "NVarChar");
                                sp.Value = "CAT";
                                sl.Add(sp);
                                sp = new SqlParameter("@CompName", "NVarChar");
                                sp.Value = CompName;
                                sl.Add(sp);
                                sp = new SqlParameter("@NCDoc", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f1", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f2", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f3", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Name")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Name"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f4", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f5", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f6", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f7", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f8", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("ShowOnHomePage")) == DBNull.Value)
                                    sp.Value = true;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("ShowOnHomePage"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f9", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f10", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f11", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f12", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f13", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f14", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f15", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f16", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Description")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Description"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f17", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f18", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f19", "Bit");
                                sp.Value = false;
                                sl.Add(sp);
                                sp = new SqlParameter("@f20", "DateTime");
                                sp.Value = DBNull.Value;
                                sl.Add(sp);
                            }
                            break;
                        case "ROLE":
                            {
                                sp = new SqlParameter("@NCNo", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@RecType", "NVarChar");
                                sp.Value = "ROLE";
                                sl.Add(sp);
                                sp = new SqlParameter("@CompName", "NVarChar");
                                sp.Value = CompName;
                                sl.Add(sp);
                                sp = new SqlParameter("@NCDoc", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f1", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f2", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f3", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Name")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Name"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f4", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f5", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f6", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f7", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f8", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("TaxExempt")) == DBNull.Value)
                                    sp.Value = true;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("TaxExempt"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f9", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f10", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f11", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f12", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f13", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f14", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f15", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f16", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f17", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f18", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f19", "Bit");
                                sp.Value = false;
                                sl.Add(sp);
                                sp = new SqlParameter("@f20", "DateTime");
                                sp.Value = DBNull.Value;
                                sl.Add(sp);
                            }
                            break;
                        case "CUST":
                            {
                                sp = new SqlParameter("@NCNo", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@RecType", "NVarChar");
                                sp.Value = "CUST";
                                sl.Add(sp);
                                sp = new SqlParameter("@CompName", "NVarChar");
                                sp.Value = CompName;
                                sl.Add(sp);
                                sp = new SqlParameter("@NCDoc", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f1", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("PasswordSalt")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("PasswordSalt"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f2", "Int");
                                if (reader.GetValue(reader.GetOrdinal("StateProvinceId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("StateProvinceId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f3", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Email")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Email"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f4", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f5", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f6", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f7", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f8", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("Deleted")) == DBNull.Value)
                                    sp.Value = true;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Deleted"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f9", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f10", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f11", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("FirstName")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("FirstName"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f12", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("LastName")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("LastName"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f13", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Address1")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Address1"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f14", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("City")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("City"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f15", "Int");
                                if (reader.GetValue(reader.GetOrdinal("CustomerRole_Id")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("CustomerRole_Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f16", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Password")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Password"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f17", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("ZipPostalCode")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("ZipPostalCode"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f18", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Company")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Company"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f19", "Bit");
                                sp.Value = false;
                                sl.Add(sp);
                                sp = new SqlParameter("@f20", "DateTime");
                                sp.Value = DBNull.Value;
                                sl.Add(sp);
                            }
                            break;
                        case "ITEM":
                            {
                                sp = new SqlParameter("@NCNo", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@RecType", "NVarChar");
                                sp.Value = "ITEM";
                                sl.Add(sp);
                                sp = new SqlParameter("@CompName", "NVarChar");
                                sp.Value = CompName;
                                sl.Add(sp);
                                sp = new SqlParameter("@NCDoc", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f1", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f2", "Int");
                                if (reader.GetValue(reader.GetOrdinal("CategoryId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("CategoryId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f3", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Name")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Name"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f4", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f5", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("Price")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Price"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f6", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("ProductCost")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("ProductCost"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f7", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("StockQuantity")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("StockQuantity"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f8", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("IsFeaturedProduct")) == DBNull.Value)
                                    sp.Value = true;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("IsFeaturedProduct"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f9", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f10", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f11", "VarChar");
                                //System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                                //file.WriteLine("Sku - " + reader.GetValue(reader.GetOrdinal("Sku")));
                                if (reader.GetValue(reader.GetOrdinal("Sku")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Sku"));
                                //file.Close();
                                sl.Add(sp);
                                sp = new SqlParameter("@f12", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f13", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f14", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f15", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f16", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("FullDescription")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("FullDescription"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f17", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f18", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f19", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("Deleted")) == DBNull.Value)
                                    sp.Value = false;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Deleted"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f20", "DateTime");
                                sp.Value = DBNull.Value;
                                sl.Add(sp);
                            }
                            break;
                        case "PRICE":
                            {
                                sp = new SqlParameter("@NCNo", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@RecType", "NVarChar");
                                sp.Value = "PRICE";
                                sl.Add(sp);
                                sp = new SqlParameter("@CompName", "NVarChar");
                                sp.Value = CompName;
                                sl.Add(sp);
                                sp = new SqlParameter("@NCDoc", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f1", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f2", "Int");
                                if (reader.GetValue(reader.GetOrdinal("ProductId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("ProductId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f3", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f4", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f5", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("Price")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Price"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f6", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f7", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("Quantity")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Quantity"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f8", "Bit");
                                sp.Value = true;
                                sl.Add(sp);
                                sp = new SqlParameter("@f9", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f10", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f11", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f12", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f13", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f14", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f15", "Int");
                                if (reader.GetValue(reader.GetOrdinal("CustomerRoleId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("CustomerRoleId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f16", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f17", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f18", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f19", "Bit");
                                sp.Value = false;
                                sl.Add(sp);
                                sp = new SqlParameter("@f20", "DateTime");
                                sp.Value = DBNull.Value;
                                sl.Add(sp);
                            }
                            break;
                        case "SADDR":
                            {
                                sp = new SqlParameter("@NCNo", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@RecType", "NVarChar");
                                sp.Value = "SADDR";
                                sl.Add(sp);
                                sp = new SqlParameter("@CompName", "NVarChar");
                                sp.Value = CompName;
                                sl.Add(sp);
                                sp = new SqlParameter("@NCDoc", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f1", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                int outp = 0;
                                using (SqlConnection con2 = new SqlConnection(NCAuth))
                                using (SqlCommand cmd2 = new SqlCommand("dbo.CESP_GETCUST", con2))
                                {
                                    cmd2.CommandType = CommandType.StoredProcedure;
                                    con2.Open();
                                    sp = new SqlParameter("@AddrId", "Int");
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                    cmd2.Parameters.Add(sp);
                                    SqlParameter output = new SqlParameter("@RetNo", SqlDbType.Int);
                                    output.Direction = ParameterDirection.Output;
                                    cmd2.Parameters.Add(output);
                                    try
                                    {
                                        cmd2.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt");
                                        file.WriteLine(ex.ToString());
                                        file.Close();
                                    }
                                    sp = new SqlParameter("@f2", "Int");
                                    if (output.Value == DBNull.Value)
                                        sp.Value = 0;
                                    else
                                        sp.Value = (int)output.Value;
                                }

                                sl.Add(sp);
                                sp = new SqlParameter("@f3", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f4", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f5", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f6", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f7", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f8", "Bit");
                                sp.Value = true;
                                sl.Add(sp);
                                sp = new SqlParameter("@f9", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f10", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f11", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("FirstName")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("FirstName"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f12", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("LastName")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("LastName"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f13", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Address1")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Address1"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f14", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("City")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("City"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f15", "Int");
                                if (reader.GetValue(reader.GetOrdinal("StateProvinceId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("StateProvinceId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f16", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f17", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("ZipPostalCode")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("ZipPostalCode"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f18", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Company")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Company"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f19", "Bit");
                                sp.Value = false;
                                sl.Add(sp);
                                sp = new SqlParameter("@f20", "DateTime");
                                sp.Value = DBNull.Value;
                                sl.Add(sp);
                            }
                            break;
                        case "SH":
                            {
                                sp = new SqlParameter("@NCNo", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@RecType", "NVarChar");
                                sp.Value = "SH";
                                sl.Add(sp);
                                sp = new SqlParameter("@CompName", "NVarChar");
                                sp.Value = CompName;
                                sl.Add(sp);
                                sp = new SqlParameter("@NCDoc", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f1", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f2", "Int");
                                if (reader.GetValue(reader.GetOrdinal("CustomerId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("CustomerId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f3", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f4", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("OrderSubtotalExclTax")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("OrderSubtotalExclTax"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f5", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("OrderSubtotalInclTax")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("OrderSubtotalInclTax"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f6", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("OrderShippingInclTax")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("OrderShippingInclTax"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f7", "Decimal");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f8", "Bit");
                                sp.Value = true;
                                sl.Add(sp);
                                sp = new SqlParameter("@f9", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("CustomerCurrencyCode")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("CustomerCurrencyCode"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f10", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("CurrencyRate")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("CurrencyRate"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f11", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("FirstName")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("FirstName"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f12", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("LastName")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("LastName"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f13", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Address1")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Address1"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f14", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("City")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("City"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f15", "Int");
                                if (reader.GetValue(reader.GetOrdinal("ShippingAddressId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("ShippingAddressId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f16", "VarChar");
                                sp.Value = "";
                                sl.Add(sp); //change
                                sp = new SqlParameter("@f17", "VarChar");//paymmethod  PaymentMethodSystemName
                                sp.Value = reader.GetValue(reader.GetOrdinal("PaymentMethodSystemName"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f18", "VarChar");//tracking
                                sp.Value = reader.GetValue(reader.GetOrdinal("ShippingStatusId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f19", "Bit");//paid
                                if (reader.GetValue(reader.GetOrdinal("PaymentStatusId")).ToString() == "30")
                                {
                                    sp.Value = true;
                                }
                                else
                                {
                                    sp.Value = false;
                                }
                                sl.Add(sp);
                                sp = new SqlParameter("@f20", "DateTime");//shipment date
                                sp.Value = DBNull.Value;
                                sl.Add(sp);
                            }
                            break;
                        case "SHL":
                            {
                                sp = new SqlParameter("@NCNo", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("Id"));
                                sl.Add(sp);
                                sp = new SqlParameter("@RecType", "NVarChar");
                                sp.Value = "SHL";
                                sl.Add(sp);
                                sp = new SqlParameter("@CompName", "NVarChar");
                                sp.Value = CompName;
                                sl.Add(sp);
                                sp = new SqlParameter("@NCDoc", "Int");
                                sp.Value = reader.GetValue(reader.GetOrdinal("OrderId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f1", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f2", "Int");
                                if (reader.GetValue(reader.GetOrdinal("ProductId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("ProductId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f3", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("AttributeDescription")) == DBNull.Value)
                                    sp.Value = "";
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("AttributeDescription"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f4", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("PriceExclTax")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("PriceExclTax"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f5", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("PriceInclTax")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("PriceInclTax"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f6", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("UnitPriceExclTax")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("UnitPriceExclTax"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f7", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("Quantity")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("Quantity"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f8", "Bit");
                                sp.Value = true;
                                sl.Add(sp);
                                sp = new SqlParameter("@f9", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f10", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("UnitPriceInclTax")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("UnitPriceInclTax"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f11", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("ParentItemId")) == DBNull.Value)
                                    sp.Value = 0;
                                else
                                    sp.Value = reader.GetValue(reader.GetOrdinal("ParentItemId"));
                                sl.Add(sp);
                                sp = new SqlParameter("@f12", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f13", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f14", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f15", "Int");
                                sp.Value = 0;
                                sl.Add(sp);
                                sp = new SqlParameter("@f16", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f17", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f18", "VarChar");
                                sp.Value = "";
                                sl.Add(sp);
                                sp = new SqlParameter("@f19", "Bit");
                                sp.Value = false;
                                sl.Add(sp);
                                sp = new SqlParameter("@f20", "DateTime");
                                sp.Value = DBNull.Value;
                                sl.Add(sp);
                            }
                            break;
                    }
                    #endregion
                    if (RecType == "SH")
                        FromSql("SEL", "SADDR", DT, CompName, Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ShippingAddressId"))));
                    ToSql("SET", RecType, DT, CompName, sl, "");
                }
                catch (Exception ex)                
                {
                    System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                    file.WriteLine("Error: possibly sending data to NAV: ToSql:" + DT + " Rec/Dir/DocNo:" + RecType + "/" + Direction + "/" + DocNo);
                    file.WriteLine(ex.ToString());
                    file.Close();
                    //return 0;
                }                
            }
        }
    }
}
//return Success;
}
public static void SetQty(string BT)
 {
     try
     {
         using (SqlConnection con = new SqlConnection(NCAuth))
         using (SqlCommand cmd = new SqlCommand("dbo.CESP_QTYUPD", con))
         {
             cmd.CommandType = CommandType.StoredProcedure;
             con.Open();
             using (StreamReader sr = new StreamReader(BT))
             {
                 SqlParameter sp = new SqlParameter("@csv", "NVarChar");
                 sp.Value = sr.ReadToEnd();
                 cmd.Parameters.Add(sp);
             }
             try
             {
                 cmd.ExecuteNonQuery();
             }
             catch (Exception ex)
             {
                 System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
                 file.WriteLine("Error: Qty Sent to SQL ");
                 file.WriteLine(ex.ToString());
                 file.Close();
                 //return 0;
             }
         }
     }
    catch (Exception ex)
     {
         System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
         file.WriteLine("Error: Qty Related: While Qty file is opened");
         file.WriteLine(ex.ToString());
         file.Close();
         //return 0;
     }
}
public static void CreatePic(string RecType, string Code, string photoFilePath, bool Deleteall)
 {
    byte[] photo;
    FileStream stream = new FileStream(
    photoFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader reader = new BinaryReader(stream);
        photo = reader.ReadBytes((int)stream.Length);
        reader.Close();
        stream.Close();
        string FirstLine = Deleteall ? "DELETE FROM Picture; DELETE FROM Product_Picture_Mapping; UPDATE Category SET PictureId = 1; " : "";
        string Mappings = RecType == "ITEM" ? 
            "INSERT INTO [dbo].[Product_Picture_Mapping] (ProductId, PictureId, "+
            "DisplayOrder) "+
            "Values(@Id, SCOPE_IDENTITY(), 1 "+
            ")"
             : 
            "IF EXISTS ( SELECT 1 FROM [dbo].[Category]  WHERE [Id] = @Id ) BEGIN "+
            "UPDATE [dbo].[Category] " +
            "SET [PictureId] = SCOPE_IDENTITY() " +
            "WHERE  [Id] = @Id END";
        using (SqlConnection connection = new SqlConnection(
            NCAuth)){
            SqlCommand command = new SqlCommand(
            FirstLine+
            "INSERT INTO [dbo].[Picture] (PictureBinary, MimeType, "+
            "SeoFilename, IsNew) "+
            "Values(@Photo, 'image/bmp', @Seo, 1 "+
            ") "+Mappings
            , connection); 
          command.Parameters.Add("@Photo",
      SqlDbType.Image, photo.Length).Value = photo;
          command.Parameters.Add("@Seo", 
      SqlDbType.NVarChar, 10).Value = DBNull.Value;
          command.Parameters.Add("@Id", 
      SqlDbType.Int).Value = Code;
          connection.Open();
          command.ExecuteNonQuery();
      }
}
#region //paramsmain
public static void GetParams(ref List<SqlParameter> sl, string RecType, string Direction, string ProcText, DateTime DT)
{
SqlParameter sp;
string[] navvalues = ProcText.Split(',');
if (Direction == "DEL")
{
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[1]);
            sl.Add(sp);
}
if (Direction == "PROC")
switch (RecType)
{
case "CAT" : {
        sp = new SqlParameter("@CategoryTemplateId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@MetaKeywords", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@MetaDescription", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@MetaTitle", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@ParentCategoryId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@PictureId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@PageSize", SqlDbType.Int);
        sp.Value = Convert.ToInt32(4);
            sl.Add(sp);
        sp = new SqlParameter("@AllowCustomersToSelectPageSize", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@PageSizeOptions", SqlDbType.NVarChar);
        sp.Value = "12,8,4";
            sl.Add(sp);
        sp = new SqlParameter("@PriceRanges", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@IncludeInTopMenu", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@HasDiscountsApplied", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@SubjectToAcl", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@LimitedToStores", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@Published", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@Deleted", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@DisplayOrder", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime);
        sp.Value = DT;
            sl.Add(sp);
        sp = new SqlParameter("@UpdatedOnUtc", SqlDbType.DateTime);
        sp.Value = DT;
            sl.Add(sp);
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[0]);
            sl.Add(sp);
        sp = new SqlParameter("@Name", SqlDbType.VarChar);
        sp.Value = navvalues[1];
            sl.Add(sp);
        sp = new SqlParameter("@ShowOnHomePage", SqlDbType.Bit);
        sp.Value = navvalues[2] == "Yes" ? true : false;
            sl.Add(sp);
        sp = new SqlParameter("@Description", SqlDbType.VarChar);
        sp.Value = navvalues[3];
            sl.Add(sp);
}
 break;
 case "ROLE" : {
        sp = new SqlParameter("@FreeShipping", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@Active", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@IsSystemRole", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@SystemName", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[0]);
            sl.Add(sp);
        sp = new SqlParameter("@Name", SqlDbType.VarChar);
        sp.Value = navvalues[1];
            sl.Add(sp);
        sp = new SqlParameter("@TaxExempt", SqlDbType.Bit);
        sp.Value = navvalues[2] == "Yes" ? true : false;
            sl.Add(sp);
}
 break;
 case "CUST" : {
        sp = new SqlParameter("@CustomerGuid", "uniqueidentifier");
        sp.Value = Guid.NewGuid();
            sl.Add(sp);
        sp = new SqlParameter("@Username", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@PasswordFormatId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@AdminComment", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@IsTaxExempt", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@AffiliateId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@VendorId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@Active", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@IsSystemAccount", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@SystemName", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@LastIpAddress", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime);
        sp.Value = DT;
            sl.Add(sp);
        sp = new SqlParameter("@LastLoginDateUtc", SqlDbType.DateTime);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@LastActivityDateUtc", SqlDbType.DateTime);
        sp.Value = DT;
            sl.Add(sp);
        sp = new SqlParameter("@BillingAddress_Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@ShippingAddress_Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@CountryId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(6);
            sl.Add(sp);
        sp = new SqlParameter("@Address2", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@FaxNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[0]);
            sl.Add(sp);
        sp = new SqlParameter("@PasswordSalt", SqlDbType.VarChar);
        sp.Value = navvalues[1];
            sl.Add(sp);
        sp = new SqlParameter("@StateProvinceId", SqlDbType.Int);
        if (navvalues[2] != "")
            sp.Value = Convert.ToInt32(navvalues[2]);
        else
            sp.Value = DBNull.Value;
        sl.Add(sp);
        sp = new SqlParameter("@Email", SqlDbType.VarChar);
        sp.Value = navvalues[3];
            sl.Add(sp);
        sp = new SqlParameter("@Deleted", SqlDbType.Bit);
        sp.Value = navvalues[4] == "Yes" ? true : false;
            sl.Add(sp);
        sp = new SqlParameter("@FirstName", SqlDbType.VarChar);
        sp.Value = navvalues[5];
            sl.Add(sp);
        sp = new SqlParameter("@LastName", SqlDbType.VarChar);
        sp.Value = navvalues[6];
            sl.Add(sp);
        sp = new SqlParameter("@Address1", SqlDbType.VarChar);
        sp.Value = navvalues[7];
            sl.Add(sp);
        sp = new SqlParameter("@City", SqlDbType.VarChar);
        sp.Value = navvalues[8];
            sl.Add(sp);
        sp = new SqlParameter("@CustomerRole_Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[9]);
            sl.Add(sp);
        sp = new SqlParameter("@Password", SqlDbType.VarChar);
        sp.Value = navvalues[10];
            sl.Add(sp);
        sp = new SqlParameter("@ZipPostalCode", SqlDbType.VarChar);
        sp.Value = navvalues[11];
            sl.Add(sp);
        sp = new SqlParameter("@Company", SqlDbType.VarChar);
        sp.Value = navvalues[12];
            sl.Add(sp);
}
 break;
 case "ITEM" : {
        sp = new SqlParameter("@ProductTypeId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(5);
            sl.Add(sp);
        sp = new SqlParameter("@Sku", SqlDbType.NVarChar);
        //System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
        //file.WriteLine("Sku - " + navvalues[9]);
        //file.Close();
        sp.Value = navvalues[9];
            sl.Add(sp);
        sp = new SqlParameter("@ParentGroupedProductId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@VisibleIndividually", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@ShortDescription", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@AdminComment", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@ProductTemplateId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@VendorId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@ShowOnHomePage", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@MetaKeywords", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@MetaDescription", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@MetaTitle", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@AllowCustomerReviews", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@ApprovedRatingSum", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@NotApprovedRatingSum", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@ApprovedTotalReviews", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@NotApprovedTotalReviews", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@SubjectToAcl", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@LimitedToStores", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@ManufacturerPartNumber", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@Gtin", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@IsGiftCard", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@GiftCardTypeId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@RequireOtherProducts", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@RequiredProductIds", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@AutomaticallyAddRequiredProducts", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@IsDownload", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@DownloadId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@UnlimitedDownloads", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@MaxNumberOfDownloads", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@DownloadExpirationDays", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@DownloadActivationTypeId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@HasSampleDownload", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@SampleDownloadId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@HasUserAgreement", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@UserAgreementText", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@IsRecurring", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@RecurringCycleLength", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@RecurringCyclePeriodId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@RecurringTotalCycles", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@IsShipEnabled", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@IsFreeShipping", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@AdditionalShippingCharge", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@DeliveryDateId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(2);
            sl.Add(sp);
        sp = new SqlParameter("@WarehouseId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@IsTaxExempt", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@TaxCategoryId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@ManageInventoryMethodId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@DisplayStockAvailability", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@DisplayStockQuantity", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@MinStockQuantity", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@LowStockActivityId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@NotifyAdminForQuantityBelow", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@BackorderModeId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@AllowBackInStockSubscriptions", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@OrderMinimumQuantity", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@OrderMaximumQuantity", SqlDbType.Int);
        sp.Value = Convert.ToInt32(10000);
            sl.Add(sp);
        sp = new SqlParameter("@AllowedQuantities", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@DisableBuyButton", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@DisableWishlistButton", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@AvailableForPreOrder", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@CallForPrice", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@OldPrice", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@SpecialPrice", SqlDbType.Decimal);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@SpecialPriceStartDateTimeUtc", SqlDbType.DateTime);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@SpecialPriceEndDateTimeUtc", SqlDbType.DateTime);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@CustomerEntersPrice", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@MinimumCustomerEnteredPrice", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@MaximumCustomerEnteredPrice", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@HasTierPrices", SqlDbType.Bit);
        sp.Value = true;
            sl.Add(sp);
        sp = new SqlParameter("@HasDiscountsApplied", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@Weight", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@Length", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@Width", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@Height", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@AvailableStartDateTimeUtc", SqlDbType.DateTime);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@AvailableEndDateTimeUtc", SqlDbType.DateTime);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@DisplayOrder", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@Deleted", SqlDbType.Bit);
        sp.Value =false;
            sl.Add(sp);
        sp = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime);
        sp.Value = DT;
            sl.Add(sp);
        sp = new SqlParameter("@UpdatedOnUtc", SqlDbType.DateTime);
        sp.Value = DT;
            sl.Add(sp);
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[0]);
            sl.Add(sp);
        sp = new SqlParameter("@CategoryId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[1]);
            sl.Add(sp);
        sp = new SqlParameter("@Name", SqlDbType.VarChar);
        sp.Value = navvalues[2];
            sl.Add(sp);
        sp = new SqlParameter("@Price", SqlDbType.Decimal);
        sp.Value = navvalues[3];
            sl.Add(sp);
        sp = new SqlParameter("@ProductCost", SqlDbType.Decimal);
        sp.Value = navvalues[4];
            sl.Add(sp);
        sp = new SqlParameter("@StockQuantity", SqlDbType.Int);
        sp.Value = Math.Round(Convert.ToDecimal(navvalues[5]));
            sl.Add(sp);
        sp = new SqlParameter("@IsFeaturedProduct", SqlDbType.Bit);
        sp.Value = navvalues[6] == "Yes" ? true : false;
            sl.Add(sp);
        sp = new SqlParameter("@FullDescription", SqlDbType.VarChar);
        sp.Value = navvalues[7];
            sl.Add(sp);
        sp = new SqlParameter("@Published", SqlDbType.Bit);
        sp.Value = navvalues[8] == "No" ? true : false;
            sl.Add(sp);
}
 break;
 case "PRICE" : {
        sp = new SqlParameter("@StoreId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[0]);
            sl.Add(sp);
        sp = new SqlParameter("@ProductId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[1]);
            sl.Add(sp);
        sp = new SqlParameter("@Price", SqlDbType.Decimal);
        sp.Value = navvalues[2];
            sl.Add(sp);
        sp = new SqlParameter("@Quantity", SqlDbType.Int);
        sp.Value = Math.Round(Convert.ToDecimal(navvalues[3]));
            sl.Add(sp);
        sp = new SqlParameter("@CustomerRoleId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[4]);
            sl.Add(sp);
}
 break;
 case "SADDR" : {
        sp = new SqlParameter("@Email", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CountryId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(6);
            sl.Add(sp);
        sp = new SqlParameter("@Address2", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@FaxNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime);
        sp.Value = DT;
            sl.Add(sp);
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[0]);
            sl.Add(sp);
        sp = new SqlParameter("@Customer_Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[1]);
            sl.Add(sp);
        sp = new SqlParameter("@FirstName", SqlDbType.VarChar);
        sp.Value = navvalues[2];
            sl.Add(sp);
        sp = new SqlParameter("@LastName", SqlDbType.VarChar);
        sp.Value = navvalues[3];
            sl.Add(sp);
        sp = new SqlParameter("@Address1", SqlDbType.VarChar);
        sp.Value = navvalues[4];
            sl.Add(sp);
        sp = new SqlParameter("@City", SqlDbType.VarChar);
        sp.Value = navvalues[5];
            sl.Add(sp);
        sp = new SqlParameter("@StateProvinceId", SqlDbType.Int);
        if (navvalues[6] != "")
            sp.Value = Convert.ToInt32(navvalues[6]);
        else
            sp.Value = DBNull.Value;
        sl.Add(sp);
        sp = new SqlParameter("@ZipPostalCode", SqlDbType.VarChar);
        sp.Value = navvalues[7];
            sl.Add(sp);
        sp = new SqlParameter("@Company", SqlDbType.VarChar);
        sp.Value = navvalues[8];
            sl.Add(sp);
}
 break;
 case "SH" : {
        sp = new SqlParameter("@OrderGuid", "uniqueidentifier");
        sp.Value = Guid.NewGuid();
            sl.Add(sp);
        sp = new SqlParameter("@StoreId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@BillingAddressId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(3);
            sl.Add(sp);
        sp = new SqlParameter("@CustomerTaxDisplayTypeId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(10);
            sl.Add(sp);
        sp = new SqlParameter("@VatNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@OrderSubTotalDiscountInclTax", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@OrderSubTotalDiscountExclTax", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@OrderShippingExclTax", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@PaymentMethodAdditionalFeeInclTax", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@PaymentMethodAdditionalFeeExclTax", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@TaxRates", SqlDbType.NVarChar);
        sp.Value = "0:0;   ";
            sl.Add(sp);
        sp = new SqlParameter("@OrderTax", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@OrderDiscount", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@OrderTotal", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@RefundedAmount", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@RewardPointsWereAdded", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@CheckoutAttributeDescription", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CheckoutAttributesXml", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@CustomerLanguageId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(1);
            sl.Add(sp);
        sp = new SqlParameter("@AffiliateId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@CustomerIp", SqlDbType.NVarChar);
        sp.Value = "::1";
            sl.Add(sp);
        sp = new SqlParameter("@AllowStoringCreditCardNumber", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@CardType", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CardName", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CardNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@MaskedCreditCardNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CardCvv2", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CardExpirationMonth", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CardExpirationYear", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@AuthorizationTransactionId", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@AuthorizationTransactionCode", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@AuthorizationTransactionResult", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@CaptureTransactionId", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@CaptureTransactionResult", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@SubscriptionTransactionId", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@PurchaseOrderNumber", SqlDbType.NVarChar);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@PaidDateUtc", SqlDbType.DateTime);
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@ShippingRateComputationMethodSystemName", SqlDbType.NVarChar);
        sp.Value = "Shipping.FixedRate";
            sl.Add(sp);
        sp = new SqlParameter("@Deleted", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime);
        sp.Value = DT;
            sl.Add(sp);
        sp = new SqlParameter("@Email", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@Company", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@CountryId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(6);
            sl.Add(sp);
        sp = new SqlParameter("@Address2", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@ZipPostalCode", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@FaxNumber", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[0]);
            sl.Add(sp);
        sp = new SqlParameter("@CustomerId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[1]);
            sl.Add(sp);
        sp = new SqlParameter("@OrderSubtotalExclTax", SqlDbType.Decimal);
        sp.Value = navvalues[2];
            sl.Add(sp);
        sp = new SqlParameter("@OrderSubtotalInclTax", SqlDbType.Decimal);
        sp.Value = navvalues[3];
            sl.Add(sp);
        sp = new SqlParameter("@OrderShippingInclTax", SqlDbType.Decimal);
        sp.Value = navvalues[4];
            sl.Add(sp);
        sp = new SqlParameter("@ShippingMethod", SqlDbType.NVarChar);
        //if (Convert.ToInt32(navvalues[4]) > 10) //need to send method instead of adjusting based on price
        sp.Value = "Australia Post";
        sl.Add(sp);
        sp = new SqlParameter("@CustomerCurrencyCode", SqlDbType.VarChar);
        sp.Value = navvalues[5];
            sl.Add(sp);
        sp = new SqlParameter("@CurrencyRate", SqlDbType.Decimal);
        sp.Value = navvalues[6];
            sl.Add(sp);
        sp = new SqlParameter("@FirstName", SqlDbType.VarChar);
        sp.Value = navvalues[7];
            sl.Add(sp);
        sp = new SqlParameter("@LastName", SqlDbType.VarChar);
        sp.Value = navvalues[8];
            sl.Add(sp);
        sp = new SqlParameter("@Address1", SqlDbType.VarChar);
        sp.Value = navvalues[9];
            sl.Add(sp);
        sp = new SqlParameter("@City", SqlDbType.VarChar);
        sp.Value = navvalues[10];
            sl.Add(sp);
        sp = new SqlParameter("@ShippingAddressId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[11]);
            sl.Add(sp);
        sp = new SqlParameter("@StateProvinceId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[12]);
            sl.Add(sp);
        sp = new SqlParameter("@PaymentMethodSystemName", SqlDbType.NVarChar);
        sp.Value = navvalues[13];
            sl.Add(sp);
       
        if (navvalues[14] == "No") //not posted, not shipped
        {
            sp = new SqlParameter("@OrderStatusId", SqlDbType.Int);
            sp.Value = Convert.ToInt32(20);
            sl.Add(sp);
            sp = new SqlParameter("@ShippingStatusId", SqlDbType.Int);
            sp.Value = Convert.ToInt32(20);
            sl.Add(sp);
            sp = new SqlParameter("@PaymentStatusId", SqlDbType.Int);
            sp.Value = Convert.ToInt32(10);
            sl.Add(sp);
        }
        else
        {
            sp = new SqlParameter("@OrderStatusId", SqlDbType.Int);
            sp.Value = Convert.ToInt32(30);
            sl.Add(sp);
            sp = new SqlParameter("@ShippingStatusId", SqlDbType.Int);
            sp.Value = Convert.ToInt32(30);
            sl.Add(sp);
            sp = new SqlParameter("@PaymentStatusId", SqlDbType.Int);
            sp.Value = Convert.ToInt32(30);
            sl.Add(sp);
        }
        //Shipping Details, add tracking number and ShipmentDate
        sp = new SqlParameter("@TrackingNumber", SqlDbType.VarChar);
        sp.Value = navvalues[15];
        sl.Add(sp);
        if (navvalues[16] != "") {
        sp = new SqlParameter("@ShippedDate", SqlDbType.DateTime);
        sp.Value = Convert.ToDateTime(navvalues[16]).ToUniversalTime();
            sl.Add(sp);
        sp = new SqlParameter("@DeliveryDate", SqlDbType.DateTime);
        sp.Value = Convert.ToDateTime(navvalues[16]).ToUniversalTime().AddDays(5);
            sl.Add(sp);
        } else {
        sp = new SqlParameter("@ShippedDate", "datetime");
        sp.Value = DBNull.Value;
            sl.Add(sp);
        sp = new SqlParameter("@DeliveryDate", "datetime");
        sp.Value = DBNull.Value;
            sl.Add(sp);
        }

}
 break;
    case "SHL" : {
        sp = new SqlParameter("@OrderItemGuid", "uniqueidentifier");
        sp.Value = Guid.NewGuid();
            sl.Add(sp);
        sp = new SqlParameter("@DiscountAmountInclTax", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@DiscountAmountExclTax", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@OriginalProductCost", SqlDbType.Decimal);
        sp.Value = 0;
            sl.Add(sp);
        sp = new SqlParameter("@AttributesXml", SqlDbType.NVarChar);
        sp.Value = "";
            sl.Add(sp);
        sp = new SqlParameter("@DownloadCount", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@IsDownloadActivated", SqlDbType.Bit);
        sp.Value = false;
            sl.Add(sp);
        sp = new SqlParameter("@LicenseDownloadId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(0);
            sl.Add(sp);
        sp = new SqlParameter("@ItemWeight", SqlDbType.Decimal);
        sp.Value = 1;
            sl.Add(sp);
        sp = new SqlParameter("@Id", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[0]);
            sl.Add(sp);
        sp = new SqlParameter("@OrderId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[1]);
            sl.Add(sp);
        sp = new SqlParameter("@ProductId", SqlDbType.Int);
        sp.Value = Convert.ToInt32(navvalues[2]);
            sl.Add(sp);
        sp = new SqlParameter("@AttributeDescription", SqlDbType.VarChar);
        sp.Value = navvalues[3];
            sl.Add(sp);
        sp = new SqlParameter("@PriceExclTax", SqlDbType.Decimal);
        sp.Value = navvalues[4];
            sl.Add(sp);
        sp = new SqlParameter("@PriceInclTax", SqlDbType.Decimal);
        sp.Value = navvalues[5];
            sl.Add(sp);
        sp = new SqlParameter("@UnitPriceExclTax", SqlDbType.Decimal);
        sp.Value = navvalues[6];
            sl.Add(sp);
        sp = new SqlParameter("@Quantity", SqlDbType.Int);
        sp.Value = Math.Round(Convert.ToDecimal(navvalues[7]));
            sl.Add(sp);
        sp = new SqlParameter("@UnitPriceInclTax", SqlDbType.Decimal);
        sp.Value = navvalues[8];
            sl.Add(sp);
        if (navvalues[9] != "") {
                sp = new SqlParameter("@ExpiryDate", SqlDbType.DateTime);
                sp.Value = Convert.ToDateTime(navvalues[9]).ToUniversalTime();
                    sl.Add(sp);
        } else {
                sp = new SqlParameter("@ExpiryDate", "datetime");
                sp.Value = DBNull.Value;
                    sl.Add(sp);
        }
 break;
 }
    case "COMP":
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter("C:\\Install\\NCObjs\\NCErrorLog.txt", true);
            sp = new SqlParameter("@Id", SqlDbType.Int);
            sp.Value = Convert.ToInt32(navvalues[0]);
            //file.WriteLine(Convert.ToInt32(navvalues[0]));
            sl.Add(sp);
            sp = new SqlParameter("@ProductId", SqlDbType.Int);
            sp.Value = Convert.ToInt32(navvalues[1]);
            //file.WriteLine(Convert.ToInt32(navvalues[1]));
            sl.Add(sp);
            sp = new SqlParameter("@ProductName", SqlDbType.VarChar);
            sp.Value = navvalues[2];
            //file.WriteLine(navvalues[2]);
            sl.Add(sp);
            sp = new SqlParameter("@CustomerId", SqlDbType.Int);
            sp.Value = Convert.ToInt32(navvalues[3]);
            //file.WriteLine(Convert.ToInt32(navvalues[3]));
            sl.Add(sp);
            sp = new SqlParameter("@ParentItemId", SqlDbType.VarChar);
            sp.Value = navvalues[4];
            //file.WriteLine(navvalues[4]);
            sl.Add(sp);
            //sp = new SqlParameter("@Active", SqlDbType.Bit);
            //if (navvalues[5] == "No")
            //    sp.Value = false;
            //else
            //    sp.Value = true;
            //sl.Add(sp);   
            sp = new SqlParameter("@Quantity", SqlDbType.Int);
            sp.Value = Math.Round(Convert.ToDecimal(navvalues[5]));
            //file.WriteLine(navvalues[5]);
            sl.Add(sp);
            if (navvalues[6] != "")
            {
                //file.WriteLine("Positive EPD if" + navvalues[6]);
                sp = new SqlParameter("@ExpiryDate", SqlDbType.DateTime);
                sp.Value = Convert.ToDateTime(navvalues[6]).ToUniversalTime();
                sl.Add(sp);
            }
            else
            {
                //file.WriteLine("Negative EPD if" + navvalues[6]);
                sp = new SqlParameter("@ExpiryDate", "datetime");
                sp.Value = DBNull.Value;
                sl.Add(sp);
            }
            sp = new SqlParameter("@ParentItemName", SqlDbType.VarChar);
            sp.Value = navvalues[7];
            //file.WriteLine(navvalues[7]);
            sl.Add(sp);
            if (navvalues[8] != "")
            {
                //file.WriteLine("Positive PIN if " + navvalues[8]);
                sp = new SqlParameter("@ParentItemSerial", SqlDbType.VarChar);
                sp.Value = navvalues[8];
                sl.Add(sp);
            }
            else
            {
                //file.WriteLine("Negative PIN if " + navvalues[8]);
                sp = new SqlParameter("@ParentItemSerial", "");
                sp.Value = DBNull.Value;
                sl.Add(sp);
            }
            sp = new SqlParameter("@ParentLocation", SqlDbType.VarChar);
            sp.Value = navvalues[9];
            //file.WriteLine(navvalues[9]);
            sl.Add(sp);
            sp = new SqlParameter("@ParentSubLocation", SqlDbType.VarChar);
            sp.Value = navvalues[10];
            //file.WriteLine(navvalues[10]);
            sl.Add(sp);
            //file.Close();
        } break;
}
}
}
#endregion
#endregion
