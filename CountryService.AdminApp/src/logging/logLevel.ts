/** 
 * Classifications that indicate the severity. 
 * 
 * @remarks
 * https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-9.0#log-level
 * 
 * */
export enum LogLevel {
    Trace = 0,
    Debug = 1,
    Info = 2,
    Warn = 3,
    Error = 4,
    None = 6
}
