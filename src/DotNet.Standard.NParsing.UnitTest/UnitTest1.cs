using System;
using System.Data.SqlClient;
using System.Reflection;
using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.NParsing.DbUtilities;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DotNet.Standard.NParsing.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var emp = (EmployeInfo)ObModel.Create(typeof(EmployeInfo));
            emp.Name = "abc";
            /*var cc = new EmployeInfo();
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
            var list = dal.SqlText("SELECT e.ID Id, e.Name Name, e.DepartmentID DepartmentId, d.ID Department_Id, d.Name Department_Name FROM Employes e LEFT JOIN Departments d ON d.ID=e.DepartmentID WHERE e.ID=@ID", new SqlParameter("@ID", 1)).ToList(); 
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
                /*.GroupBy(o => new
                {
                    o.Gender,
                    o.Department.Id,
                    o.Department.Name,
                })
                .Select(o => new
                {
                    Age = o.Avg( k => k.Age).As(k => k.Age),
                    Name = o.Min(k => k.Id)
                })
                .Where(o => o.Age > 20)
                .OrderByDescending(o => new
                {
                    o.Name,
                    o.Department.Id
                })
                .OrderBy(o => o.Department.Name)*/
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

        [TestMethod]
        public void TestMethod2()
        {
            var emp = new EmployeInfo().Of();
            emp.Age = 18;
            var employe = new Employe("Employes").Of();
            /*var join = employe.Join();
            join.AddJoin(o => o.Department);*/
            ObJoinBase join = null;
            join = (ObJoinBase)employe.Join(join).AddJoin(o => o.Department);
            //join.AddJoin(o => o.Department.Director);
            var sort = employe.OrderBy(o => new
            {
                o.Age,
                o.Gender
            });
            //sort.AddOrderBy(o => o.Gender);
            var group = employe.GroupBy(o => new
            {
                o.Age,
                o.Gender
            });
            //group.AddGroupBy(o => o.Gender);
            var dal = employe.Helper<EmployeInfo, Employe>("database=NSmart.Demo01;server=.;uid=sa;pwd=1;Pooling=true;Connection Timeout=300;", "DotNet.Standard.NParsing.SQLServer");

            var a = dal.Query(join, group, sort).ToList();


        }
    }
}
