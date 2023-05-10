using ClosedXML.Excel;
using GPS.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading;

namespace DXApplicationFDA.Infra.Services
{
    public static class ExportDataService
    {
        public static ExportDto ExportDataToFile<T>(List<T> dataSource, DataTable header, string title, DataTable lookups=null )
        {
                var rtl = Thread.CurrentThread.CurrentCulture.Name == "en" ? false : true;
                var workbook = new XLWorkbook { RightToLeft = rtl };
                var worksheet = workbook.Worksheets.Add(title);
                int currentRow = 1;
                int Cell = 1;
                if (dataSource.Count == 0)
                {
                    dataSource.Add((T)Activator.CreateInstance(typeof(T)));
                }
                Type TheType = dataSource[0].GetType();
                PropertyInfo[] properties = TheType.GetProperties();

                foreach (DataColumn column in header.Columns)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (column.ColumnName == properties[i].Name)
                        {
                            worksheet.Cell(5, Cell).Value = header.Rows[0][properties[i].Name];
                            worksheet.Cell(5, Cell).Style.Fill.BackgroundColor = XLColor.FromArgb(125, 109, 48);
                            worksheet.Cell(5, Cell).Style.Font.FontColor = XLColor.White;
                            worksheet.Cell(5, Cell).Style.Font.SetBold();
                            worksheet.Cell(5, Cell).Style.Font.FontSize = 14;
                            worksheet.Cell(5, Cell).Style.Alignment.Horizontal = rtl ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;

                            currentRow = 5;
                            for (int j = 0; j < dataSource.Count; j++)
                            {
                                currentRow++;
                                if (dataSource[j].GetType().GetProperty(properties[i].Name).PropertyType == typeof(DateTime) || dataSource[j].GetType().GetProperty(properties[i].Name).PropertyType == typeof(DateTime?))
                                {
                                    var date = dataSource[j].GetType().GetProperty(properties[i].Name).GetValue(dataSource[j], null);
                                    worksheet.Cell(currentRow, Cell).Value = date != null ? Convert.ToDateTime(date).ToString("yyyy-MM-dd") : null;
                                    worksheet.Cell(currentRow, Cell).Style.DateFormat.Format = "yyyy-MM-dd";
                                }
                                else if (dataSource[j].GetType().GetProperty(properties[i].Name).PropertyType == typeof(List<string>))
                                {
                                    List<string> options = lookups.Rows[0].Field<List<string>>(properties[i].Name);
                                    if (options.Count > 0)
                                    {

                                    }
                                    else
                                    {
                                        worksheet.Cell(currentRow, Cell).Value = null;
                                    }
                                }
                                else
                                {
                                    worksheet.Cell(currentRow, Cell).Value = dataSource[j].GetType().GetProperty(properties[i].Name).GetValue(dataSource[j], null);
                                }

                                worksheet.Cell(currentRow, Cell).Style.Alignment.Horizontal = rtl ? XLAlignmentHorizontalValues.Right : XLAlignmentHorizontalValues.Left;

                            }
                            Cell++;
                        }
                    }
                }
                worksheet.Cell(2, 5).Value = title;
                worksheet.Cell(2, 5).Style.Font.FontSize = 17;
                worksheet.Cell(2, 5).Style.Font.SetBold();

                var range = worksheet.Range(5, 1, currentRow, Cell - 1);
                var table = range.CreateTable();
                table.Theme = XLTableTheme.TableStyleMedium25;

                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                var content = stream.ToArray();

                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return new ExportDto { Content = content, ContentType = contentType };
           
        }
    }
}
