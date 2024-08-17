namespace Skillitory.Api.DataStore.Common.Enumerations;

public enum AuditLogTypeEnum
{
    SignIn = 1,
    SignOut,
    ForgotPassword,
    ResetPassword,
    NewUserRegistered,
    NewUserEmailVerified
}
