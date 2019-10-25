using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DotNet.Standard.NParsing.ComponentModel;
using DotNet.Standard.NParsing.Factory;
using DotNet.Standard.NParsing.Interface;

namespace DotNet.Standard.NParsing.UnitTest
{
    [ObModel(Name = "Employes")]
    public class EmployeBaseInfo : ObModelBase
    {
        private int _id;

        /// <summary>
        /// 员工编号
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
        /// 员工名称
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
    }

    public class EmployeBase : ObTermBase
    {
        public EmployeBase(Type modelType) : base(modelType)
        {
        }

        public EmployeBase(Type modelType, string rename) : base(modelType, rename)
        {
        }

        public EmployeBase(ObTermBase parent, MethodBase currentMethod) : base(typeof(EmployeBaseInfo), parent, currentMethod)
        {
        }

        public EmployeBase(Type modelType, ObTermBase parent, MethodBase currentMethod) : base(modelType, parent, currentMethod)
        {
        }

        /// <summary>
        /// 员工编号
        /// </summary>		
        public ObProperty Id
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        /// <summary>
        /// 员工名称
        /// </summary>		
        public ObProperty Name
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }
    }

    [ObModel(Name = "Employes")]
    public class EmployeInfo : EmployeBaseInfo
    {

        private int _gender;

        /// <summary>
        /// 性别
        /// </summary>	
        [ObProperty(Name = "Gender", Length = 4, Nullable = false)]
        public int Gender
        {
            get { return _gender; }
            set
            {
                SetPropertyValid(MethodBase.GetCurrentMethod());
                _gender = value;
            }
        }

        private DateTime _createtime;

        /// <summary>
        /// 创建时间
        /// </summary>	
        [ObProperty(Name = "CreateTime", Length = 8, Precision = 3, Nullable = false, Modifiable = false)]
        public DateTime CreateTime
        {
            get { return _createtime; }
            set
            {
                if (value.Year >= 1900 && value.Month > 0 && value.Day > 0)
                {
                    SetPropertyValid(MethodBase.GetCurrentMethod());
                }
                _createtime = value;
            }
        }

        private bool _dimission;

        /// <summary>
        /// Dimission
        /// </summary>	
        [ObProperty(Name = "Dimission", Length = 1, Nullable = false)]
        public bool Dimission
        {
            get { return _dimission; }
            set
            {
                SetPropertyValid(MethodBase.GetCurrentMethod());
                _dimission = value;
            }
        }

        private int _departmentid;

        /// <summary>
        /// 部门编号
        /// </summary>
        [ObConstraint(ObConstraint.ForeignKey, Refclass = typeof(DepartmentInfo), Refproperty = "Id")]
        [ObProperty(Name = "DepartmentID", Length = 4, Nullable = true)]
        public int DepartmentId
        {
            get { return _departmentid; }
            set
            {
                SetPropertyValid(MethodBase.GetCurrentMethod());
                _departmentid = value;
            }
        }

        private int _age;

        /// <summary>
        /// 年龄
        /// </summary>	
        [ObProperty(Name = "Age", Length = 4, Nullable = false)]
        public int Age
        {
            get { return _age; }
            set
            {
                SetPropertyValid(MethodBase.GetCurrentMethod());
                _age = value;
            }
        }

        public DepartmentInfo Department { get; set; }
    }

    public class Employe : EmployeBase
    {
        public Employe() : base(typeof(EmployeInfo))
        {
        }

        public Employe(string rename) : base(typeof(EmployeInfo), rename)
        {
        }

        public Employe(ObTermBase parent, MethodBase currentMethod) : base(typeof(EmployeInfo), parent, currentMethod)
        {
        }

        /// <summary>
        /// 性别
        /// </summary>		
        public ObProperty Gender
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        /// <summary>
        /// 创建时间
        /// </summary>		
        public ObProperty CreateTime
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        /// <summary>
        /// Dimission
        /// </summary>		
        public ObProperty Dimission
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        /// <summary>
        /// 部门编号
        /// </summary>		
        public ObProperty DepartmentId
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        /// <summary>
        /// 年龄
        /// </summary>		
        public ObProperty Age
        {
            get { return GetProperty(MethodBase.GetCurrentMethod()); }
        }

        public Department Department
        {
            get { return new Department(this, MethodBase.GetCurrentMethod()); }
        }

    }

    public interface IEmployeInfo
    {
        int Age { get; set; }
    }
}
