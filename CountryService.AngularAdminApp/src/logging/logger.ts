import { Injectable } from '@angular/core';
import { LogLevel } from './logLevel'; 
import { environment } from '../environments/environment';

@Injectable({
    providedIn: 'root',
})
export class Logger 
{
    public logLevel: LogLevel = environment.logLevel;
    
    constructor() { } 
    
    public trace(msg: string): void 
    {
        this.log(LogLevel.Trace, msg);
    }
    
    public debug(msg: string): void 
    {
        this.log(LogLevel.Debug, msg);
    }
    
    public info(msg: string): void 
    {
        this.log(LogLevel.Info, msg);
    } 
    
    public warn(msg: string): void 
    {
        this.log(LogLevel.Warn, msg);
    } 
    
    public error(msg: string): void 
    {
        this.log(LogLevel.Error, msg);
    } 
    
    private log(logLevel: LogLevel, msg: string): void 
    {
        if (this.logLevel != LogLevel.None && this.logLevel <= logLevel)
        {
            switch (logLevel) 
            {
                case LogLevel.Trace:
                    return console.trace(`level=trace ${msg}`);

                case LogLevel.Debug:
                    return console.debug(`level=debug ${msg}`);

                case LogLevel.Info:
                    return console.info(`level=info ${msg}`);

                case LogLevel.Warn:
                    return console.warn(`level=warn ${msg}`);

                case LogLevel.Error:
                    return console.error(`level=error ${msg} 
                        ${this.getClientInfo()}`);

                default:
                    console.warn(`Unknown log level : ${logLevel} ${msg}`);
            }
        }
    }

    private getClientInfo(): string 
    {
        return `path: ${window.location.pathname},
	        queryString: ${window.location.search},
	        screenWidth: ${window.screen.width},
	        screenHeight: ${window.screen.height},
	        cookieEnabled: ${navigator.cookieEnabled},
	        language: ${navigator.language},
	        userAgent: ${navigator.userAgent}`;
    }
}
