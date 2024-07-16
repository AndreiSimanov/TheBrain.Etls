<b>Утилита экспорта/импорта данных (Notes.md) в excel файл.</b>

Формируемый excel файл состоит из трёх колонок:
<ul>
<li>Thought Id (GUID)</li>
<li>Thought Name (string)</li>
<li>Content (Контент фапйла Notes.md)</li>
</ul>

<b>Приложение выполняет 2 команды:</b>

<b>Создать еxcel файл на основе данных приложения 'TheBrain'</b><br>
<br>
Параметры вызова команды:<br>
<ul>
  command=CreateExcelFile (Запуск указанной команды)<br>
  excelFilePath=<Путь к файлу excel><br>
  brainsFolderPath=<Путь к базе данных приложения TheBrain><br>
</ul>
Необязательные параметры:<br><br>
<ul>
  dbFileName=<Имя файла базы данных приложения TheBrain> (По умолчанию = Brain.db)<br>
  contentFileName=<Имя файла контента приложения TheBrain> (По умолчанию  = Notes.md)<br>
  logFilePath=<Путь к файлу логирования> (Default = TheBrain.Etls.log)<br>
  lang=<Установить язык приложения ru/en> (Default = ru)<br>
</ul>      
<br>
Примеры вызова:<br>
TheBrain.Etls.exe command=CreateExcelFile excelFilePath=C:\temp\test.xlsx brainsFolderPath=C:\Users\user\Brains\U01\B04<br>
<br>
TheBrain.Etls.exe command=CreateExcelFile excelFilePath=C:\temp\test.xlsx brainsFolderPath=C:\Users\user\Brains\U01\B04 lang=en<br>
<br>
<br>
<b>Загрузить контент из еxcel файла в приложение 'TheBrain'</b><br>
<br>
Параметры вызова команды:<br>
<ul>
  command=UploadFilesFromExcelFile (Запуск указанной команды)<br>
  excelFilePath=<Путь к файлу excel><br>
  brainsFolderPath=<Путь к базе данных приложения TheBrain><br>
</ul>
Необязательные параметры:<br><br>
<ul>
  dbFileName=<Имя файла базы данных приложения TheBrain> (По умолчанию = Brain.db)<br>
  contentFileName=<Имя файла контента приложения TheBrain> (По умолчанию  = Notes.md)<br>
  logFilePath=<Путь к файлу логирования> (Default = TheBrain.Etls.log)<br>
  lang=<Установить язык приложения ru/en> (Default = ru)<br>
</ul>      
<br>
Примеры вызова:<br>
TheBrain.Etls.exe command=UploadFilesFromExcelFile excelFilePath=C:\temp\test.xlsx brainsFolderPath=C:\Users\user\Brains\U01\B04<br>
<br>
TheBrain.Etls.exe command=UploadFilesFromExcelFile excelFilePath=C:\temp\test.xlsx brainsFolderPath=C:\Users\user\Brains\U01\B04 lang=en<br>
<br>
Приложение поддерживает русский и английский язык.<br>
<br>
<b>Основные библиотеки используемые в приложении:</b><br><br>

Console CommandLine

	Microsoft.Extensions.Configuration.CommandLine 8.0.0

Local DB (SQLite)

	Microsoft.EntityFrameworkCore 8.0.7
	Microsoft.EntityFrameworkCore.Proxies 8.0.7
	Microsoft.EntityFrameworkCore.Sqlite 8.0.7

Excel maker

	EPPlus 7.2.1
	
Application Log

	Serilog.Sinks.File 6.0.0


