using EmailSender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.SQLite;

namespace FileProcessor
{
    public class FileProcessor
    {
        private SQLiteConnection dbConnection { get; set; }
        private FullConfiguredEmail email { get; set; }

        public FileProcessor()
        {
            SQLiteConnection.CreateFile("MyDatabase.sqlite");
            dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
            dbConnection.Open();
            const string sql = "create table processedResult (TimeStamp varchar(20), Operation varchar(20), Result double)";
            var command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
            email = new FullConfiguredEmail();
        }

        public void Run(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                while (true)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        break;
                    }
                    var lineComponents = line.Split(';');
                    var componentsCount = lineComponents.Length;
                    if (componentsCount < 3)
                    {
                        sendEmail(string.Format("Invalid line, no operation or not enough operands - {0}", line));
                    }
                    var operation = lineComponents[componentsCount - 1];
                    processOperation(lineComponents.Take(componentsCount - 1).ToArray(), operation);
                }
            }
            const string sql = "select * from processedResult";
            var command = new SQLiteCommand(sql, dbConnection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(string.Format("TimeStamp [{0}]\tOperation [{1}]\tResult [{2}] ", reader["TimeStamp"],
                    reader["Operation"], reader["Result"]));
            }
            Console.ReadKey();
        }

        private void processOperation(IEnumerable<string> operands, string operation)
        {
            double result = 0;
            var isFirstItemProcessed = false;
            switch (operation)
            {
                case "Addition":
                    result = 0;
                    foreach (var operand in operands)
                    {
                        int op;
                        if (!int.TryParse(operand, out op))
                        {
                            sendEmail(string.Format("Invalid operator in line. - {0}", operand));
                            return;
                        }
                        result += op;
                    }
                    writeToDatabase(operation, result);
                    break;
                case "Subtraction":
                    foreach (var operand in operands)
                    {
                        int op;
                        if (!int.TryParse(operand, out op))
                        {
                            sendEmail(string.Format("Invalid operator in line. - {0}", operand));
                            return;
                        }
                        if (!isFirstItemProcessed)
                        {
                            isFirstItemProcessed = true;
                            result = op;
                            continue;
                        }
                        result -= op;
                    }
                    writeToDatabase(operation, result);
                    break;
                case "Division":
                    foreach (var operand in operands)
                    {
                        int op;
                        if (!int.TryParse(operand, out op))
                        {
                            sendEmail(string.Format("Invalid operator in line. - {0}", operand));
                            return;
                        }
                        if (!isFirstItemProcessed)
                        {
                            isFirstItemProcessed = true;
                            result = op;
                            continue;
                        }
                        if (op == 0)
                        {
                            sendEmail("Divide by zero exception in line.");
                            return;
                        }
                        result /= op;
                    }
                    writeToDatabase(operation, result);
                    break;
                case "Multiplication":
                    result = 1;
                    foreach (var operand in operands)
                    {
                        int op;
                        if (!int.TryParse(operand, out op))
                        {
                            sendEmail(string.Format("Invalid operator in line. - {0}", operand));
                            return;
                        }
                        result *= op;
                    }
                    writeToDatabase(operation, result);
                    break;
                default:
                    sendEmail(string.Format("Encountered unknown operation in file - {0}", operation));
                    break;
            }
        }

        private void writeToDatabase(string operation, double result)
        {
            if (result < 0)
            {
                sendEmail(string.Format("Result is less than zero. operation - [{0}], result [{1}]", operation, result));
            }
            string sql =string.Format("insert into processedResult (TimeStamp, Operation, Result) values ('{0}', '{1}', {2})",
                    DateTime.Now.ToString(), operation, result);
            var command = new SQLiteCommand(sql, dbConnection);
            command.ExecuteNonQuery();
        }

        private void sendEmail(string message)
        {
            Console.WriteLine(string.Format("Email Sent to John Doe. Message - {0}", message));
            email.Send("john_doe@example.com", message);
        }
    }
}
