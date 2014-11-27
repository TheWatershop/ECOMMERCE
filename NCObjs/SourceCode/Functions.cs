using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public class Functions
{
    private static string NAVAuth = "Server = ECOMMERCE; Database = WSNOPInt; User Id = SA;Password = 99226400!;";
    private static string NCAuth = "Server = ECOMMERCE; Database = NOP; User Id = SA;Password = 99226400!;";

    public static void CreatePic(string RecType, string Code, string photoFilePath, bool Deleteall)
    {
        FileStream input = new FileStream(photoFilePath, FileMode.Open, FileAccess.Read);
        BinaryReader reader = new BinaryReader(input);
        byte[] buffer = reader.ReadBytes((int) input.Length);
        reader.Close();
        input.Close();
        string str = Deleteall ? "DELETE FROM Picture; DELETE FROM Product_Picture_Mapping; UPDATE Category SET PictureId = 1; " : "";
        string str2 = (RecType == "ITEM") ? "INSERT INTO [dbo].[Product_Picture_Mapping] (ProductId, PictureId, DisplayOrder) Values(@Id, SCOPE_IDENTITY(), 1 )" : "IF EXISTS ( SELECT 1 FROM [dbo].[Category]  WHERE [Id] = @Id ) BEGIN UPDATE [dbo].[Category] SET [PictureId] = SCOPE_IDENTITY() WHERE  [Id] = @Id END";
        using (SqlConnection connection = new SqlConnection(NCAuth))
        {
            SqlCommand command = new SqlCommand(str + "INSERT INTO [dbo].[Picture] (PictureBinary, MimeType, SeoFilename, IsNew) Values(@Photo, 'image/bmp', @Seo, 1 ) " + str2, connection);
            command.Parameters.Add("@Photo", SqlDbType.Image, buffer.Length).Value = buffer;
            command.Parameters.Add("@Seo", SqlDbType.NVarChar, 10).Value = DBNull.Value;
            command.Parameters.Add("@Id", SqlDbType.Int).Value = Code;
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public static void FromSql(string Direction, string RecType, DateTime DT, string CompName, int DocNo)
    {
        if ((RecType != "ROLE") && (Direction != "Sel"))
        {
            using (SqlConnection connection = new SqlConnection(NCAuth))
            {
                using (SqlCommand command = new SqlCommand("dbo.CESP_" + RecType + "SEL", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    if ((RecType == "SHL") || (RecType == "SADDR"))
                    {
                        command.Parameters.Add(new SqlParameter("@DocNo", SqlDbType.Int)).Value = DocNo;
                    }
                    else
                    {
                        command.Parameters.Add(new SqlParameter("@LastDate", SqlDbType.DateTime)).Value = DT;
                    }
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Exception exception;
                            StreamWriter writer;
                            try
                            {
                                SqlParameter parameter;
                                List<SqlParameter> sl = new List<SqlParameter>();
                                switch (RecType)
                                {
                                    case "CAT":
                                        parameter = new SqlParameter("@NCNo", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("Id"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@RecType", "NVarChar") {
                                            Value = "CAT"
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@CompName", "NVarChar") {
                                            Value = CompName
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@NCDoc", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f1", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f2", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f3", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("Name")) != DBNull.Value)
                                        {
                                            break;
                                        }
                                        parameter.Value = "";
                                        goto Label_02ED;

                                    case "ROLE":
                                        parameter = new SqlParameter("@NCNo", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("Id"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@RecType", "NVarChar") {
                                            Value = "ROLE"
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@CompName", "NVarChar") {
                                            Value = CompName
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@NCDoc", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f1", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f2", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f3", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("Name")) != DBNull.Value)
                                        {
                                            goto Label_0749;
                                        }
                                        parameter.Value = "";
                                        goto Label_0762;

                                    case "CUST":
                                        parameter = new SqlParameter("@NCNo", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("Id"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@RecType", "NVarChar") {
                                            Value = "CUST"
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@CompName", "NVarChar") {
                                            Value = CompName
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@NCDoc", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f1", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("PasswordSalt")) != DBNull.Value)
                                        {
                                            goto Label_0B33;
                                        }
                                        parameter.Value = "";
                                        goto Label_0B4C;

                                    case "ITEM":
                                        parameter = new SqlParameter("@NCNo", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("Id"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@RecType", "NVarChar") {
                                            Value = "ITEM"
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@CompName", "NVarChar") {
                                            Value = CompName
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@NCDoc", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f1", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f2", "Int");
                                        if (reader.GetValue(reader.GetOrdinal("CategoryId")) != DBNull.Value)
                                        {
                                            goto Label_11EC;
                                        }
                                        parameter.Value = 0;
                                        goto Label_1205;

                                    case "PRICE":
                                        parameter = new SqlParameter("@NCNo", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("Id"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@RecType", "NVarChar") {
                                            Value = "PRICE"
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@CompName", "NVarChar") {
                                            Value = CompName
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@NCDoc", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f1", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f2", "Int");
                                        if (reader.GetValue(reader.GetOrdinal("ProductId")) != DBNull.Value)
                                        {
                                            goto Label_178D;
                                        }
                                        parameter.Value = 0;
                                        goto Label_17A6;

                                    case "SADDR":
                                        parameter = new SqlParameter("@NCNo", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("Id"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@RecType", "NVarChar") {
                                            Value = "SADDR"
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@CompName", "NVarChar") {
                                            Value = CompName
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@NCDoc", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f1", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        using (SqlConnection connection2 = new SqlConnection(NCAuth))
                                        {
                                            using (SqlCommand command2 = new SqlCommand("dbo.CESP_GETCUST", connection2))
                                            {
                                                command2.CommandType = CommandType.StoredProcedure;
                                                connection2.Open();
                                                parameter = new SqlParameter("@AddrId", "Int") {
                                                    Value = reader.GetValue(reader.GetOrdinal("Id"))
                                                };
                                                command2.Parameters.Add(parameter);
                                                SqlParameter parameter2 = new SqlParameter("@RetNo", SqlDbType.Int) {
                                                    Direction = ParameterDirection.Output
                                                };
                                                command2.Parameters.Add(parameter2);
                                                try
                                                {
                                                    command2.ExecuteNonQuery();
                                                }
                                                catch (Exception exception1)
                                                {
                                                    exception = exception1;
                                                    writer = new StreamWriter(@"C:\Install\NCObjs\NCErrorLog.txt");
                                                    writer.WriteLine(exception.ToString());
                                                    writer.Close();
                                                }
                                                parameter = new SqlParameter("@f2", "Int");
                                                if (parameter2.Value == DBNull.Value)
                                                {
                                                    parameter.Value = 0;
                                                }
                                                else
                                                {
                                                    parameter.Value = (int) parameter2.Value;
                                                }
                                            }
                                        }
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f3", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f4", "Decimal") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f5", "Decimal") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f6", "Decimal") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f7", "Decimal") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f8", "Bit") {
                                            Value = true
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f9", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f10", "Decimal") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f11", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("FirstName")) == DBNull.Value)
                                        {
                                            parameter.Value = "";
                                        }
                                        else
                                        {
                                            parameter.Value = reader.GetValue(reader.GetOrdinal("FirstName"));
                                        }
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f12", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("LastName")) == DBNull.Value)
                                        {
                                            parameter.Value = "";
                                        }
                                        else
                                        {
                                            parameter.Value = reader.GetValue(reader.GetOrdinal("LastName"));
                                        }
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f13", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("Address1")) == DBNull.Value)
                                        {
                                            parameter.Value = "";
                                        }
                                        else
                                        {
                                            parameter.Value = reader.GetValue(reader.GetOrdinal("Address1"));
                                        }
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f14", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("City")) == DBNull.Value)
                                        {
                                            parameter.Value = "";
                                        }
                                        else
                                        {
                                            parameter.Value = reader.GetValue(reader.GetOrdinal("City"));
                                        }
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f15", "Int");
                                        if (reader.GetValue(reader.GetOrdinal("StateProvinceId")) == DBNull.Value)
                                        {
                                            parameter.Value = 0;
                                        }
                                        else
                                        {
                                            parameter.Value = reader.GetValue(reader.GetOrdinal("StateProvinceId"));
                                        }
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f16", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f17", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("ZipPostalCode")) == DBNull.Value)
                                        {
                                            parameter.Value = "";
                                        }
                                        else
                                        {
                                            parameter.Value = reader.GetValue(reader.GetOrdinal("ZipPostalCode"));
                                        }
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f18", "VarChar");
                                        if (reader.GetValue(reader.GetOrdinal("Company")) == DBNull.Value)
                                        {
                                            parameter.Value = "";
                                        }
                                        else
                                        {
                                            parameter.Value = reader.GetValue(reader.GetOrdinal("Company"));
                                        }
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f19", "Bit") {
                                            Value = false
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f20", "DateTime") {
                                            Value = DBNull.Value
                                        };
                                        sl.Add(parameter);
                                        goto Label_2E0C;

                                    case "SH":
                                        parameter = new SqlParameter("@NCNo", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("Id"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@RecType", "NVarChar") {
                                            Value = "SH"
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@CompName", "NVarChar") {
                                            Value = CompName
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@NCDoc", "Int") {
                                            Value = 0
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f1", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f2", "Int");
                                        if (reader.GetValue(reader.GetOrdinal("CustomerId")) != DBNull.Value)
                                        {
                                            goto Label_22C7;
                                        }
                                        parameter.Value = 0;
                                        goto Label_22E0;

                                    case "SHL":
                                        parameter = new SqlParameter("@NCNo", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("Id"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@RecType", "NVarChar") {
                                            Value = "SHL"
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@CompName", "NVarChar") {
                                            Value = CompName
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@NCDoc", "Int") {
                                            Value = reader.GetValue(reader.GetOrdinal("OrderId"))
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f1", "VarChar") {
                                            Value = ""
                                        };
                                        sl.Add(parameter);
                                        parameter = new SqlParameter("@f2", "Int");
                                        if (reader.GetValue(reader.GetOrdinal("ProductId")) != DBNull.Value)
                                        {
                                            goto Label_297C;
                                        }
                                        parameter.Value = 0;
                                        goto Label_2995;

                                    default:
                                        goto Label_2E0C;
                                }
                                parameter.Value = reader.GetValue(reader.GetOrdinal("Name"));
                            Label_02ED:
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f4", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f5", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f6", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f7", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f8", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("ShowOnHomePage")) == DBNull.Value)
                                {
                                    parameter.Value = true;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("ShowOnHomePage"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f9", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f10", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f11", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f12", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f13", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f14", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f15", "Int") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f16", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Description")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Description"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f17", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f18", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f19", "Bit") {
                                    Value = false
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f20", "DateTime") {
                                    Value = DBNull.Value
                                };
                                sl.Add(parameter);
                                goto Label_2E0C;
                            Label_0749:
                                parameter.Value = reader.GetValue(reader.GetOrdinal("Name"));
                            Label_0762:
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f4", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f5", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f6", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f7", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f8", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("TaxExempt")) == DBNull.Value)
                                {
                                    parameter.Value = true;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("TaxExempt"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f9", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f10", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f11", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f12", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f13", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f14", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f15", "Int") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f16", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f17", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f18", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f19", "Bit") {
                                    Value = false
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f20", "DateTime") {
                                    Value = DBNull.Value
                                };
                                sl.Add(parameter);
                                goto Label_2E0C;
                            Label_0B33:
                                parameter.Value = reader.GetValue(reader.GetOrdinal("PasswordSalt"));
                            Label_0B4C:
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f2", "Int");
                                if (reader.GetValue(reader.GetOrdinal("StateProvinceId")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("StateProvinceId"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f3", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Email")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Email"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f4", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f5", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f6", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f7", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f8", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("Deleted")) == DBNull.Value)
                                {
                                    parameter.Value = true;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Deleted"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f9", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f10", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f11", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("FirstName")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("FirstName"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f12", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("LastName")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("LastName"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f13", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Address1")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Address1"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f14", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("City")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("City"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f15", "Int");
                                if (reader.GetValue(reader.GetOrdinal("CustomerRole_Id")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("CustomerRole_Id"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f16", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Password")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Password"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f17", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("ZipPostalCode")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("ZipPostalCode"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f18", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Company")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Company"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f19", "Bit") {
                                    Value = false
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f20", "DateTime") {
                                    Value = DBNull.Value
                                };
                                sl.Add(parameter);
                                goto Label_2E0C;
                            Label_11EC:
                                parameter.Value = reader.GetValue(reader.GetOrdinal("CategoryId"));
                            Label_1205:
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f3", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Name")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Name"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f4", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f5", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("Price")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Price"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f6", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("ProductCost")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("ProductCost"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f7", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("StockQuantity")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("StockQuantity"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f8", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("IsFeaturedProduct")) == DBNull.Value)
                                {
                                    parameter.Value = true;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("IsFeaturedProduct"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f9", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f10", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f11", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f12", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f13", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f14", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f15", "Int") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f16", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("FullDescription")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("FullDescription"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f17", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f18", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f19", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("Deleted")) == DBNull.Value)
                                {
                                    parameter.Value = false;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Deleted"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f20", "DateTime") {
                                    Value = DBNull.Value
                                };
                                sl.Add(parameter);
                                goto Label_2E0C;
                            Label_178D:
                                parameter.Value = reader.GetValue(reader.GetOrdinal("ProductId"));
                            Label_17A6:
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f3", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f4", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f5", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("Price")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Price"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f6", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f7", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("Quantity")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Quantity"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f8", "Bit") {
                                    Value = true
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f9", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f10", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f11", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f12", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f13", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f14", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f15", "Int");
                                if (reader.GetValue(reader.GetOrdinal("CustomerRoleId")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("CustomerRoleId"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f16", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f17", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f18", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f19", "Bit") {
                                    Value = false
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f20", "DateTime") {
                                    Value = DBNull.Value
                                };
                                sl.Add(parameter);
                                goto Label_2E0C;
                            Label_22C7:
                                parameter.Value = reader.GetValue(reader.GetOrdinal("CustomerId"));
                            Label_22E0:
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f3", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f4", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("OrderSubtotalExclTax")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("OrderSubtotalExclTax"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f5", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("OrderSubtotalInclTax")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("OrderSubtotalInclTax"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f6", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("OrderShippingInclTax")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("OrderShippingInclTax"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f7", "Decimal") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f8", "Bit") {
                                    Value = true
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f9", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("CustomerCurrencyCode")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("CustomerCurrencyCode"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f10", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("CurrencyRate")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("CurrencyRate"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f11", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("FirstName")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("FirstName"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f12", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("LastName")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("LastName"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f13", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("Address1")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Address1"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f14", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("City")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("City"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f15", "Int");
                                if (reader.GetValue(reader.GetOrdinal("ShippingAddressId")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("ShippingAddressId"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f16", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f17", "VarChar") {
                                    Value = reader.GetValue(reader.GetOrdinal("PaymentMethodSystemName"))
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f18", "VarChar") {
                                    Value = reader.GetValue(reader.GetOrdinal("ShippingStatusId"))
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f19", "Bit");
                                if (reader.GetValue(reader.GetOrdinal("PaymentStatusId")).ToString() == "30")
                                {
                                    parameter.Value = true;
                                }
                                else
                                {
                                    parameter.Value = false;
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f20", "DateTime") {
                                    Value = DBNull.Value
                                };
                                sl.Add(parameter);
                                goto Label_2E0C;
                            Label_297C:
                                parameter.Value = reader.GetValue(reader.GetOrdinal("ProductId"));
                            Label_2995:
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f3", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("AttributeDescription")) == DBNull.Value)
                                {
                                    parameter.Value = "";
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("AttributeDescription"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f4", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("PriceExclTax")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("PriceExclTax"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f5", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("PriceInclTax")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("PriceInclTax"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f6", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("UnitPriceExclTax")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("UnitPriceExclTax"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f7", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("Quantity")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("Quantity"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f8", "Bit") {
                                    Value = true
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f9", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f10", "Decimal");
                                if (reader.GetValue(reader.GetOrdinal("UnitPriceInclTax")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("UnitPriceInclTax"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f11", "VarChar");
                                if (reader.GetValue(reader.GetOrdinal("ParentItemId")) == DBNull.Value)
                                {
                                    parameter.Value = 0;
                                }
                                else
                                {
                                    parameter.Value = reader.GetValue(reader.GetOrdinal("ParentItemId"));
                                }
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f12", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f13", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f14", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f15", "Int") {
                                    Value = 0
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f16", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f17", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f18", "VarChar") {
                                    Value = ""
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f19", "Bit") {
                                    Value = false
                                };
                                sl.Add(parameter);
                                parameter = new SqlParameter("@f20", "DateTime") {
                                    Value = DBNull.Value
                                };
                                sl.Add(parameter);
                            Label_2E0C:
                                if (RecType == "SH")
                                {
                                    FromSql("SEL", "SADDR", DT, CompName, Convert.ToInt32(reader.GetValue(reader.GetOrdinal("ShippingAddressId"))));
                                }
                                ToSql("SET", RecType, DT, CompName, sl, "");
                            }
                            catch (Exception exception2)
                            {
                                exception = exception2;
                                writer = new StreamWriter(@"C:\Install\NCObjs\NCErrorLog.txt", true);
                                writer.WriteLine(string.Concat(new object[] { "Error: possibly sending data to NAV: ToSql:", DT, " Rec/Dir/DocNo:", RecType, "/", Direction, "/", DocNo }));
                                writer.WriteLine(exception.ToString());
                                writer.Close();
                            }
                        }
                    }
                }
            }
        }
    }

    public static void GetParams(ref List<SqlParameter> sl, string RecType, string Direction, string ProcText, DateTime DT)
    {
        SqlParameter parameter;
        string[] strArray = ProcText.Split(new char[] { ',' });
        if (Direction == "DEL")
        {
            parameter = new SqlParameter("@Id", SqlDbType.Int) {
                Value = Convert.ToInt32(strArray[1])
            };
            sl.Add(parameter);
        }
        if (Direction == "PROC")
        {
            switch (RecType)
            {
                case "CAT":
                    parameter = new SqlParameter("@CategoryTemplateId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MetaKeywords", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MetaDescription", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MetaTitle", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ParentCategoryId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PictureId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PageSize", SqlDbType.Int) {
                        Value = Convert.ToInt32(4)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AllowCustomersToSelectPageSize", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PageSizeOptions", SqlDbType.NVarChar) {
                        Value = "12,8,4"
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PriceRanges", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IncludeInTopMenu", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@HasDiscountsApplied", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SubjectToAcl", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LimitedToStores", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Published", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Deleted", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DisplayOrder", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime) {
                        Value = DT
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@UpdatedOnUtc", SqlDbType.DateTime) {
                        Value = DT
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Name", SqlDbType.VarChar) {
                        Value = strArray[1]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ShowOnHomePage", SqlDbType.Bit) {
                        Value = (strArray[2] == "Yes") ? true : false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Description", SqlDbType.VarChar) {
                        Value = strArray[3]
                    };
                    sl.Add(parameter);
                    break;

                case "ROLE":
                    parameter = new SqlParameter("@FreeShipping", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Active", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsSystemRole", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SystemName", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Name", SqlDbType.VarChar) {
                        Value = strArray[1]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@TaxExempt", SqlDbType.Bit) {
                        Value = (strArray[2] == "Yes") ? true : false
                    };
                    sl.Add(parameter);
                    break;

                case "CUST":
                    parameter = new SqlParameter("@CustomerGuid", "uniqueidentifier") {
                        Value = Guid.NewGuid()
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Username", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PasswordFormatId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AdminComment", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsTaxExempt", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AffiliateId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@VendorId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Active", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsSystemAccount", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SystemName", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LastIpAddress", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime) {
                        Value = DT
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LastLoginDateUtc", SqlDbType.DateTime) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LastActivityDateUtc", SqlDbType.DateTime) {
                        Value = DT
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@BillingAddress_Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ShippingAddress_Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CountryId", SqlDbType.Int) {
                        Value = Convert.ToInt32(6)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Address2", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@FaxNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PasswordSalt", SqlDbType.VarChar) {
                        Value = strArray[1]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@StateProvinceId", SqlDbType.Int);
                    if (!(strArray[2] != ""))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = Convert.ToInt32(strArray[2]);
                    }
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Email", SqlDbType.VarChar) {
                        Value = strArray[3]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Deleted", SqlDbType.Bit) {
                        Value = (strArray[4] == "Yes") ? true : false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@FirstName", SqlDbType.VarChar) {
                        Value = strArray[5]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LastName", SqlDbType.VarChar) {
                        Value = strArray[6]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Address1", SqlDbType.VarChar) {
                        Value = strArray[7]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@City", SqlDbType.VarChar) {
                        Value = strArray[8]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerRole_Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[9])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Password", SqlDbType.VarChar) {
                        Value = strArray[10]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ZipPostalCode", SqlDbType.VarChar) {
                        Value = strArray[11]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Company", SqlDbType.VarChar) {
                        Value = strArray[12]
                    };
                    sl.Add(parameter);
                    return;

                case "ITEM":
                    parameter = new SqlParameter("@ProductTypeId", SqlDbType.Int) {
                        Value = Convert.ToInt32(5)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Sku", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ParentGroupedProductId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@VisibleIndividually", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ShortDescription", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AdminComment", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ProductTemplateId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@VendorId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ShowOnHomePage", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MetaKeywords", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MetaDescription", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MetaTitle", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AllowCustomerReviews", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ApprovedRatingSum", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@NotApprovedRatingSum", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ApprovedTotalReviews", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@NotApprovedTotalReviews", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SubjectToAcl", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LimitedToStores", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ManufacturerPartNumber", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Gtin", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsGiftCard", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@GiftCardTypeId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@RequireOtherProducts", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@RequiredProductIds", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AutomaticallyAddRequiredProducts", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsDownload", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DownloadId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@UnlimitedDownloads", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MaxNumberOfDownloads", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DownloadExpirationDays", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DownloadActivationTypeId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@HasSampleDownload", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SampleDownloadId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@HasUserAgreement", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@UserAgreementText", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsRecurring", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@RecurringCycleLength", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@RecurringCyclePeriodId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@RecurringTotalCycles", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsShipEnabled", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsFreeShipping", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AdditionalShippingCharge", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DeliveryDateId", SqlDbType.Int) {
                        Value = Convert.ToInt32(2)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@WarehouseId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsTaxExempt", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@TaxCategoryId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ManageInventoryMethodId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DisplayStockAvailability", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DisplayStockQuantity", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MinStockQuantity", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LowStockActivityId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@NotifyAdminForQuantityBelow", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@BackorderModeId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AllowBackInStockSubscriptions", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderMinimumQuantity", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderMaximumQuantity", SqlDbType.Int) {
                        Value = Convert.ToInt32(0x2710)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AllowedQuantities", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DisableBuyButton", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DisableWishlistButton", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AvailableForPreOrder", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CallForPrice", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OldPrice", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SpecialPrice", SqlDbType.Decimal) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SpecialPriceStartDateTimeUtc", SqlDbType.DateTime) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SpecialPriceEndDateTimeUtc", SqlDbType.DateTime) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerEntersPrice", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MinimumCustomerEnteredPrice", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MaximumCustomerEnteredPrice", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@HasTierPrices", SqlDbType.Bit) {
                        Value = true
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@HasDiscountsApplied", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Weight", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Length", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Width", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Height", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AvailableStartDateTimeUtc", SqlDbType.DateTime) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AvailableEndDateTimeUtc", SqlDbType.DateTime) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DisplayOrder", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Deleted", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime) {
                        Value = DT
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@UpdatedOnUtc", SqlDbType.DateTime) {
                        Value = DT
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CategoryId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[1])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Name", SqlDbType.VarChar) {
                        Value = strArray[2]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Price", SqlDbType.Decimal) {
                        Value = strArray[3]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ProductCost", SqlDbType.Decimal) {
                        Value = strArray[4]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@StockQuantity", SqlDbType.Int) {
                        Value = Math.Round(Convert.ToDecimal(strArray[5]))
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsFeaturedProduct", SqlDbType.Bit) {
                        Value = (strArray[6] == "Yes") ? true : false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@FullDescription", SqlDbType.VarChar) {
                        Value = strArray[7]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Published", SqlDbType.Bit) {
                        Value = (strArray[8] == "No") ? true : false
                    };
                    sl.Add(parameter);
                    break;

                case "PRICE":
                    parameter = new SqlParameter("@StoreId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ProductId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[1])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Price", SqlDbType.Decimal) {
                        Value = strArray[2]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Quantity", SqlDbType.Int) {
                        Value = Math.Round(Convert.ToDecimal(strArray[3]))
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerRoleId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[4])
                    };
                    sl.Add(parameter);
                    break;

                case "SADDR":
                    parameter = new SqlParameter("@Email", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CountryId", SqlDbType.Int) {
                        Value = Convert.ToInt32(6)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Address2", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@FaxNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime) {
                        Value = DT
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Customer_Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[1])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@FirstName", SqlDbType.VarChar) {
                        Value = strArray[2]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LastName", SqlDbType.VarChar) {
                        Value = strArray[3]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Address1", SqlDbType.VarChar) {
                        Value = strArray[4]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@City", SqlDbType.VarChar) {
                        Value = strArray[5]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@StateProvinceId", SqlDbType.Int);
                    if (!(strArray[6] != ""))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    else
                    {
                        parameter.Value = Convert.ToInt32(strArray[6]);
                    }
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ZipPostalCode", SqlDbType.VarChar) {
                        Value = strArray[7]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Company", SqlDbType.VarChar) {
                        Value = strArray[8]
                    };
                    sl.Add(parameter);
                    return;

                case "SH":
                    parameter = new SqlParameter("@OrderGuid", "uniqueidentifier") {
                        Value = Guid.NewGuid()
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@StoreId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@BillingAddressId", SqlDbType.Int) {
                        Value = Convert.ToInt32(3)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerTaxDisplayTypeId", SqlDbType.Int) {
                        Value = Convert.ToInt32(10)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@VatNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderSubTotalDiscountInclTax", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderSubTotalDiscountExclTax", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderShippingExclTax", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PaymentMethodAdditionalFeeInclTax", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PaymentMethodAdditionalFeeExclTax", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@TaxRates", SqlDbType.NVarChar) {
                        Value = "0:0; \x00a0\x00a0"
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderTax", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderDiscount", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderTotal", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@RefundedAmount", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@RewardPointsWereAdded", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CheckoutAttributeDescription", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CheckoutAttributesXml", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerLanguageId", SqlDbType.Int) {
                        Value = Convert.ToInt32(1)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AffiliateId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerIp", SqlDbType.NVarChar) {
                        Value = "::1"
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AllowStoringCreditCardNumber", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CardType", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CardName", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CardNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@MaskedCreditCardNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CardCvv2", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CardExpirationMonth", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CardExpirationYear", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AuthorizationTransactionId", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AuthorizationTransactionCode", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AuthorizationTransactionResult", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CaptureTransactionId", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CaptureTransactionResult", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@SubscriptionTransactionId", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PurchaseOrderNumber", SqlDbType.NVarChar) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PaidDateUtc", SqlDbType.DateTime) {
                        Value = DBNull.Value
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ShippingRateComputationMethodSystemName", SqlDbType.NVarChar) {
                        Value = "Shipping.FixedRate"
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Deleted", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CreatedOnUtc", SqlDbType.DateTime) {
                        Value = DT
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Email", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Company", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CountryId", SqlDbType.Int) {
                        Value = Convert.ToInt32(6)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Address2", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ZipPostalCode", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PhoneNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@FaxNumber", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[1])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderSubtotalExclTax", SqlDbType.Decimal) {
                        Value = strArray[2]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderSubtotalInclTax", SqlDbType.Decimal) {
                        Value = strArray[3]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderShippingInclTax", SqlDbType.Decimal) {
                        Value = strArray[4]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ShippingMethod", SqlDbType.NVarChar) {
                        Value = "Australia Post"
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerCurrencyCode", SqlDbType.VarChar) {
                        Value = strArray[5]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CurrencyRate", SqlDbType.Decimal) {
                        Value = strArray[6]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@FirstName", SqlDbType.VarChar) {
                        Value = strArray[7]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LastName", SqlDbType.VarChar) {
                        Value = strArray[8]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Address1", SqlDbType.VarChar) {
                        Value = strArray[9]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@City", SqlDbType.VarChar) {
                        Value = strArray[10]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ShippingAddressId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[11])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@StateProvinceId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[12])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PaymentMethodSystemName", SqlDbType.NVarChar) {
                        Value = strArray[13]
                    };
                    sl.Add(parameter);
                    if (!(strArray[14] == "No"))
                    {
                        parameter = new SqlParameter("@OrderStatusId", SqlDbType.Int) {
                            Value = Convert.ToInt32(30)
                        };
                        sl.Add(parameter);
                        parameter = new SqlParameter("@ShippingStatusId", SqlDbType.Int) {
                            Value = Convert.ToInt32(30)
                        };
                        sl.Add(parameter);
                        parameter = new SqlParameter("@PaymentStatusId", SqlDbType.Int) {
                            Value = Convert.ToInt32(30)
                        };
                        sl.Add(parameter);
                    }
                    else
                    {
                        parameter = new SqlParameter("@OrderStatusId", SqlDbType.Int) {
                            Value = Convert.ToInt32(20)
                        };
                        sl.Add(parameter);
                        parameter = new SqlParameter("@ShippingStatusId", SqlDbType.Int) {
                            Value = Convert.ToInt32(20)
                        };
                        sl.Add(parameter);
                        parameter = new SqlParameter("@PaymentStatusId", SqlDbType.Int) {
                            Value = Convert.ToInt32(10)
                        };
                        sl.Add(parameter);
                    }
                    parameter = new SqlParameter("@TrackingNumber", SqlDbType.VarChar) {
                        Value = strArray[15]
                    };
                    sl.Add(parameter);
                    if (strArray[0x10] != "")
                    {
                        parameter = new SqlParameter("@ShippedDate", SqlDbType.DateTime) {
                            Value = Convert.ToDateTime(strArray[0x10]).ToUniversalTime()
                        };
                        sl.Add(parameter);
                        parameter = new SqlParameter("@DeliveryDate", SqlDbType.DateTime) {
                            Value = Convert.ToDateTime(strArray[0x10]).ToUniversalTime().AddDays(5.0)
                        };
                        sl.Add(parameter);
                    }
                    else
                    {
                        parameter = new SqlParameter("@ShippedDate", "datetime") {
                            Value = DBNull.Value
                        };
                        sl.Add(parameter);
                        parameter = new SqlParameter("@DeliveryDate", "datetime") {
                            Value = DBNull.Value
                        };
                        sl.Add(parameter);
                    }
                    return;

                case "SHL":
                    parameter = new SqlParameter("@OrderItemGuid", "uniqueidentifier") {
                        Value = Guid.NewGuid()
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DiscountAmountInclTax", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DiscountAmountExclTax", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OriginalProductCost", SqlDbType.Decimal) {
                        Value = 0
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AttributesXml", SqlDbType.NVarChar) {
                        Value = ""
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@DownloadCount", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@IsDownloadActivated", SqlDbType.Bit) {
                        Value = false
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@LicenseDownloadId", SqlDbType.Int) {
                        Value = Convert.ToInt32(0)
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ItemWeight", SqlDbType.Decimal) {
                        Value = 1
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@OrderId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[1])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ProductId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[2])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@AttributeDescription", SqlDbType.VarChar) {
                        Value = strArray[3]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PriceExclTax", SqlDbType.Decimal) {
                        Value = strArray[4]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@PriceInclTax", SqlDbType.Decimal) {
                        Value = strArray[5]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@UnitPriceExclTax", SqlDbType.Decimal) {
                        Value = strArray[6]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Quantity", SqlDbType.Int) {
                        Value = Math.Round(Convert.ToDecimal(strArray[7]))
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@UnitPriceInclTax", SqlDbType.Decimal) {
                        Value = strArray[8]
                    };
                    sl.Add(parameter);
                    if (!(strArray[9] != ""))
                    {
                        parameter = new SqlParameter("@ExpiryDate", "datetime") {
                            Value = DBNull.Value
                        };
                        sl.Add(parameter);
                        break;
                    }
                    parameter = new SqlParameter("@ExpiryDate", SqlDbType.DateTime) {
                        Value = Convert.ToDateTime(strArray[9]).ToUniversalTime()
                    };
                    sl.Add(parameter);
                    break;

                case "COMP":
                    parameter = new SqlParameter("@Id", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[0])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@CustomerId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[1])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ProductId", SqlDbType.Int) {
                        Value = Convert.ToInt32(strArray[2])
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ProductName", SqlDbType.VarChar) {
                        Value = strArray[3]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ParentItemId", SqlDbType.VarChar) {
                        Value = strArray[4]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@Quantity", SqlDbType.Int) {
                        Value = Math.Round(Convert.ToDecimal(strArray[5]))
                    };
                    sl.Add(parameter);
                    if (!(strArray[6] != ""))
                    {
                        parameter = new SqlParameter("@ExpiryDate", "datetime") {
                            Value = DBNull.Value
                        };
                        sl.Add(parameter);
                    }
                    else
                    {
                        parameter = new SqlParameter("@ExpiryDate", SqlDbType.DateTime) {
                            Value = Convert.ToDateTime(strArray[6]).ToUniversalTime()
                        };
                        sl.Add(parameter);
                    }
                    parameter = new SqlParameter("@ParentItemName", SqlDbType.VarChar) {
                        Value = strArray[7]
                    };
                    sl.Add(parameter);
                    parameter = new SqlParameter("@ParentItemSerial", SqlDbType.VarChar) {
                        Value = strArray[8]
                    };
                    sl.Add(parameter);
                    return;
            }
        }
    }

    public static void SetQty(string BT)
    {
        Exception exception;
        StreamWriter writer;
        try
        {
            using (SqlConnection connection = new SqlConnection(NCAuth))
            {
                using (SqlCommand command = new SqlCommand("dbo.CESP_QTYUPD", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (StreamReader reader = new StreamReader(BT))
                    {
                        SqlParameter parameter = new SqlParameter("@csv", "NVarChar") {
                            Value = reader.ReadToEnd()
                        };
                        command.Parameters.Add(parameter);
                    }
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception exception1)
                    {
                        exception = exception1;
                        writer = new StreamWriter(@"C:\Install\NCObjs\NCErrorLog.txt", true);
                        writer.WriteLine("Error: Qty Sent to SQL ");
                        writer.WriteLine(exception.ToString());
                        writer.Close();
                    }
                }
            }
        }
        catch (Exception exception2)
        {
            exception = exception2;
            writer = new StreamWriter(@"C:\Install\NCObjs\NCErrorLog.txt", true);
            writer.WriteLine("Error: Qty Related: While Qty file is opened");
            writer.WriteLine(exception.ToString());
            writer.Close();
        }
    }

    public static void ToSql(string Direction, string RecType, DateTime DT, string CompName, List<SqlParameter> sl, string BT)
    {
        Exception exception;
        StreamWriter writer;
        string str = "";
        try
        {
            using (SqlConnection connection = new SqlConnection((Direction == "SET") ? NAVAuth : NCAuth))
            {
                using (SqlCommand command = new SqlCommand((Direction == "SET") ? "dbo.CESP_NAVSET" : ("dbo.CESP_" + RecType + Direction), connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    if (Direction == "PROC")
                    {
                        SqlParameter current;
                        string str2 = "";
                        string str3 = "";
                        using (StreamReader reader = new StreamReader(BT))
                        {
                            string[] strArray = reader.ReadToEnd().Split(new char[] { '*' });
                            foreach (string str4 in strArray)
                            {
                                try
                                {
                                    str = str4;
                                    command.Parameters.Clear();
                                    sl.Clear();
                                    string[] strArray2 = str4.Split(new char[] { ',' });
                                    str3 = strArray2[strArray2.Length - 1];
                                    GetParams(ref sl, RecType, Direction, str4, DT);
                                    using (List<SqlParameter>.Enumerator enumerator = sl.GetEnumerator())
                                    {
                                        while (enumerator.MoveNext())
                                        {
                                            current = enumerator.Current;
                                            command.Parameters.Add(current);
                                        }
                                    }
                                    SqlParameter parameter2 = new SqlParameter("@RetNo", SqlDbType.Int) {
                                        Direction = ParameterDirection.Output
                                    };
                                    command.Parameters.Add(parameter2);
                                    command.ExecuteNonQuery();
                                    if (strArray2[0] == "0")
                                    {
                                        object obj2 = str2;
                                        str2 = string.Concat(new object[] { obj2, str3, ",", parameter2.Value, "," });
                                    }
                                }
                                catch (Exception exception1)
                                {
                                    exception = exception1;
                                    writer = new StreamWriter(@"C:\Install\NCObjs\NCErrorLog.txt", true);
                                    writer.WriteLine(string.Concat(new object[] { "Error: Possible SQL: ToSql: ", DT, " Rec/Dir/BT:", RecType, "/", Direction, "/", BT, ":Line:", str }));
                                    writer.WriteLine(exception.ToString());
                                    writer.Close();
                                }
                            }
                        }
                        using (SqlConnection connection2 = new SqlConnection(NAVAuth))
                        {
                            using (SqlCommand command2 = new SqlCommand("dbo.CESP_MAPUPD", connection2))
                            {
                                command2.CommandType = CommandType.StoredProcedure;
                                connection2.Open();
                                current = new SqlParameter("@csv", "NVarChar") {
                                    Value = str2
                                };
                                command2.Parameters.Add(current);
                                command2.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        foreach (SqlParameter parameter in sl)
                        {
                            command.Parameters.Add(parameter);
                        }
                        command.ExecuteNonQuery();
                        if ((RecType == "SH") && (Direction == "SET"))
                        {
                            foreach (SqlParameter parameter in sl)
                            {
                                if (parameter.ParameterName == "@NCNo")
                                {
                                    FromSql("SEL", "SHL", DT, CompName, (int) parameter.Value);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception exception2)
        {
            exception = exception2;
            writer = new StreamWriter(@"C:\Install\NCObjs\NCErrorLog.txt", true);
            writer.WriteLine(string.Concat(new object[] { "Error: Possibly File Related: ToSql:  ", DT, " Rec/Dir/BT:", RecType, "/", Direction, "/", BT, ":Line:", str }));
            writer.WriteLine(exception.ToString());
            writer.Close();
        }
    }
}

