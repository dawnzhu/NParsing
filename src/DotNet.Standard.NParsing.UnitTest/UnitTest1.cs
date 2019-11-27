using System;
using System.Data.SqlClient;
using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNet.Standard.NParsing.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            /*var cc = new EmployeInfo().Of();
            cc.Gender = 1;*/
            /*var aa = new Employe().Of();
            var bb = aa.Department;
            var cc = new EmployeInfo().Of();
            cc.Age = 1;
            cc.Department = new DepartmentInfo().Of();
            cc.Department.Name = "abc";*/
            var employe = new Employe("Employes").Of();
            var dal = employe.Helper<EmployeInfo, Employe>("database=NSmart.Demo01;server=.;uid=sa;pwd=1;Pooling=true;Connection Timeout=300;", "DotNet.Standard.NParsing.SQLServer");
            /*var dal = ObHelper.Create<EmployeInfo, Employe>("database=NSmart.Demo01;server=.;uid=sa;pwd=1;Pooling=true;Connection Timeout=300;", "DotNet.Standard.NParsing.SQLServer");
            var list = dal.SqlText("SELECT * FROM Employes WHERE ID=@ID", new SqlParameter("@ID", 1)).ToList();*/
            //var dal = ObHelper.Create<EmployeInfo, Employe>(new Employe().Proxy(), "database=NSmart.Demo01;server=.;uid=sa;pwd=1;Pooling=true;Connection Timeout=300;", "DotNet.Standard.NParsing.SQLServer");
            /*try
            {
                var emp = new EmployeInfo().Of();
                emp.Age = 25;
                dal.Update(emp, o => o.Id == 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }*/
            var query = dal
                .Where(o => o.DepartmentId.In(1, 2, 3))
                .GroupBy(o => new
                {
                    o.Gender,
                    o.Department.Id,
                    o.Department.Name,
                })
                .Select(o => new
                {
                    Age = o.Avg(k => k.Age).As(k => k.Age),
                    Name = o.Min(k => k.Id)
                })
                .Where(o => o.Age > 20)
                .OrderByDescending(o => new
                {
                    o.Name,
                    o.Department.Id
                })
                .OrderBy(o => o.Department.Name)
                .Join(o => new
                {
                    o.Department,
                    o.Department.Director
                });
            //var a = (ObParameterBase) query.ObParameter;
            var list = query.ToList();

            /*var query = dal.GroupBy(o => o.Id).OrderBy(o => o.Id);
            var list = query.ToList(2, 2, out var count);
            var a = list;*/

        }
    }
}
