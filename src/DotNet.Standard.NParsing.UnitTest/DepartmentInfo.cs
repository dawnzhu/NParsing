using System.Reflection;
using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.UnitTest
{
    /// <summary>
    /// 部门实体类
    /// </summary>
    [ObModel(Name = "Departments")]
    public class DepartmentInfo : ObModelBase
    {
        private int _id;
        /// <summary>
        /// 部门编号
        /// </summary>	
        [ObConstraint(ObConstraint.PrimaryKey)]
        [ObProperty(Name = "ID", Length = 4, Nullable = false)]
        public int Id
        {
            get { return _id; }
            set
            {
                SetPropertyValid(MethodBase.GetCurrentMethod());
                _id = value;
            }
        }

        private string _name;

        /// <summary>
        /// 门部名称
        /// </summary>	
        [ObProperty(Name = "Name", Length = 50, Nullable = false)]
        public string Name
        {
            get { return _name; }
            set
            {
                SetPropertyValid(MethodBase.GetCurrentMethod());
                _name = value;
            }
        }

        private int _directorid;

        /// <summary>
        /// 主管编号
        /// </summary>
        [ObConstraint(ObConstraint.ForeignKey, Refclass = typeof(EmployeBaseInfo), Refproperty = "Id")]
        [ObProperty(Name = "DirectorID", Length = 4, Nullable = true)]
        public int DirectorId
        {
            get { return _directorid; }
            set
            {
                SetPropertyValid(MethodBase.GetCurrentMethod());
                _directorid = value;
            }
        }

        public EmployeBaseInfo Director { get; set; }
    }

    /// <summary>
    /// 部门条件类
    /// </summary>	
    public class Department : ObTermBase
    {
        public Department() : base(typeof(DepartmentInfo))
        { }

        public Department(ObTermBase parent, MethodBase currentMethod) : base(typeof(DepartmentInfo), parent, currentMethod)
        { }

        /// <summary>
        /// 部门编号
        /// </summary>		
        public ObProperty Id
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        /// <summary>
        /// 部门名称
        /// </summary>		
        public ObProperty Name
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        /// <summary>
        /// 主管编号
        /// </summary>		
        public ObProperty DirectorId
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        public EmployeBase Director
        {
            get { return new EmployeBase(this, MethodBase.GetCurrentMethod());}
        }
    }
}
