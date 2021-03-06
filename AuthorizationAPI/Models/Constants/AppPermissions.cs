﻿
/// <summary>
/// Permissions are based on features an application provide. 
/// We add or remove permissions only when we change functionality of application.
/// For example, if we introduce capability of managing student's grades,
/// then new permissions "Manage student's Grades" can be added.
/// However, for current snap of application, the list of permissions
/// will never change.
/// </summary>
public class AppPermissions
{
    // Resource:Action
    // Permission:Structure:Path:Action 

    public const string STUDENT_READ = "Students:Read";
    public const string STUDENT_READ_SENSITIVE_INFO = "Students:Read:SensitiveInfo";
    //public const string STUDENT_READ = "Students:SensitiveInfo:Read";
    //public const string STUDENT_READ = "Students:SensitiveInfo:Read";
    //public const string STUDENT_READ = "Students:SensitiveInfo:*";
    //public const string STUDENT_READ = "Students:*";
    //public const string STUDENT_READ = "Students:SensitiveInfo:*";
    //public const string STUDENT_READ_SENSITIVE_INFO = "Students.Read_SENSITIVE";
    public const string STUDENT_READ_SENSITIVE_INFO2 = "I can read sensitive datA!";
    public const string STUDENT_READ1 = "MicroServiceResource:Action";
    public const string ADEItems2 = "ADEItems.Read";
    public const string ADEItems3 = "ADEItems.Read";
    public const string STUDENT_READ4 = "Students.Read";
    public const string STUDENT_READ5 = "Students.Read";
    public const string STUDENT_READ6 = "Students.Read";
    public const string STUDENT_READ7 = "Students.Read";
    
    public const string MANAGE_ROLES = "Manage application roles";  //can CRUD roles.

    public const string MANAGE_PERMISSIONS = "Manage Permissions";  //can grant/revoke permissions to certain role, or certain user

    public const string MANAGE_USER = "Manage(Create/Edit/Delete/View) User";

    public const string VIEW_ADMINISTRATOR_PROFILES = "View Administrators' Profiles";
    public const string MANAGE_ADMINISTRATOR_PROFILE = "Manage (Create/Edit/Delete/View) Administrator profile";

    public const string VIEW_STUDENT_PROFILES = "View student profiles";
    public const string MANAGE_STUDENT_PROFILE = "Manage (Create/Edit/Delete) Student profile";

    public const string VIEW_OWN_STUDENT_PROFILE = "View own student profile";
    public const string VIEW_OWN_ADMIN_PROFILE = "View own administrator profile";
}
