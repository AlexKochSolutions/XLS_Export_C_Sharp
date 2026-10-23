# XLS_Export_C_Sharp
A program that imports data from a mdb database file and reading a excel template and writing the database data to a new xls file

This uses the .Net Library NPOI, see https://github.com/nissl-lab/npoi

appsettings.json scheme:

{
  "AppSettings": {
    "Databasepath": $DATABSEFILEPATH,
    "XLS_Read": $XLSREADPATH,
    "XLS_Write": $XLSWRITEPATH
  }
}
