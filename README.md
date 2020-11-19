基于.NET Standard开发的跨平台ORM框架
=============================

### **主要更新功能(V2.0.0)**
  
### 不创建条件类，实现以lambda表达式方式的增删改查
`IObHelper<Model> dal = ObHelper.Create<Model>(connectionString, providerName);`  

#### 1).查询  
`IList<Model> list = dal.Where(o => o.Name.Contains("a")).GroupBy(o => o.Name).Select(o => new {Id = o.Max(k => k.Id)}).OrderBy(o => o.Name).ToList();`  

#### 2).增加  
`dal.Add(new Model(){Id = 1, Name="abc"});`  

#### 3).更新  
`dal.Update(new Model(){Name=""}, o => o.Id == 1);`  

#### 4).删除  
`dal.Delete(o => o.Id == 1);`  

### **主要更新功能(V1.2.0)**
  
### 1.删除和更新数据允许多表关联  
`ObJoin`  

### 2.增加多个内置函数  
```
Replace
SubString
IndexOf
ToInt16
ToInt32
ToInt64
ToSingle
ToDouble
ToDecimal
ToDateTime
ToString
````

### **主要更新功能(V1.1.2)**
  
### 1.数据库执行出错，SQL语句会随错误抛出  
`ObException.CurrentExeSql`  

### 2.支持类似linq的lambda表达式书写方式  
`IObHelper<TM, TT> dal = ObHelper.Create<TM, TT>(connectionString, providerName);`  

#### 1).查询  
`IList<TM> list = dal.Where(o => o.Name.Like("a")).GroupBy(o => o.Name).Select(o => new {Id = o.Max(k => k.Id)}).OrderBy(o => o.Name).ToList();`  

#### 2).删除  
`dal.Delete(o => o.Id == 1);`  

#### 3).更新  
`dal.Update(new TM(){Name=""}, o => o.Id == 1);`  

### 3.支持数据库函数嵌套调用  
`IList<TM> list = dal.GroupBy(o => o.Name).Select(o => new {Id = o.Max(k => k.Custom("dbo.test", t => t.Id))}).ToList();`  

### 4.支持调用SQL语句返回模型对象或对象列表  
```
IObHelper<TM, TT> dal = ObHelper.Create<TM, TT>(connectionString, providerName);
TM m = dal.SqlText(sqlText, params).ToModel();
IList<TM> list = dal.SqlText(sqlText, params).ToList();
````
  
### 5.简化模型类和条件类  
例：更新数据代码  
```
/// <summary>
/// 部门模型类
/// </summary>
[ObModel(Name = "Departments")]
public class DepartmentInfo : ObModelBase
{
        /// <summary>
        /// 部门编号
        /// </summary>	
        [ObConstraint(ObConstraint.PrimaryKey)]
        [ObProperty(Name = "ID", Length = 4, Nullable = false)]
        public virtual int Id { get; set; }

        /// <summary>
        /// 门部名称
        /// </summary>	
        [ObProperty(Name = "Name", Length = 50, Nullable = false)]
        public virtual string Name { get; set; }
}

/// <summary>
/// 部门条件类
/// </summary>
public class DepartmentBase : ObTermBase
{
        public Department() : base(typeof(DepartmentInfo))
        { }

        public Department(ObTermBase parent, string rename) : base(typeof(DepartmentInfo), parent, rename)
        { }

        /// <summary>
        /// 部门编号
        /// </summary>		
        public virtual ObProperty Id { get; }

        /// <summary>
        /// 部门名称
        /// </summary>		
        public virtual ObProperty Name { get; }
}
```
  
#### 1).  
`DepartmentInfo m = new DepartmentInfo().Of();`  
或  
`DepartmentInfo m = ObModel.Create<DepartmentInfo>();`  
或  
`DepartmentInfo m = (DepartmentInfo)ObModel.Create(typeof(DepartmentInfo));`  
  
#### 2).  
`m.Name = "Name";`  
  
#### 3).  
`Department term = new Department().Of();`  
或  
`Department term = ObModel.Create<Department>(new Department());`  
或  
`Department term = (Department)ObModel.Create(typeof(Department), new Department());`  
  
#### 4).  
`IObHelper<DepartmentInfo, Department> dal = term.Helper<DepartmentInfo, Department>(connectionString, providerName);`  
或  
`IObHelper<DepartmentInfo, Department> dal = ObHelper.Create<DepartmentInfo, Department>(term, connectionString, providerName);`  
  
#### 5).  
`dal.Update(m, o => o.Id == 1);`  