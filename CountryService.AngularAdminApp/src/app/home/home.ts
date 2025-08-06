import { Component, inject } from '@angular/core';
import { environment } from '../../environments/environment';
import { Environment } from '../../models/environment';
import { Logger } from '../../logging/logger';
import { LogLevel } from '../../logging/logLevel';

@Component({
    selector: 'app-home',
    imports: [],
    templateUrl: './home.html',
    styleUrl: './home.css'
})
export class Home {

    private readonly logger: Logger = inject(Logger);
    protected env: Environment = environment;

    protected testLogger(): void 
    {
        const currentLogLevel: LogLevel = this.logger.logLevel;
     //   console.log(`Current log level : ${this.logger.logLevel}`);
        
        this.logger.logLevel = LogLevel.Trace;
     //   console.log(`log level set to : ${this.logger.logLevel}`);
         
        this.logger.trace("This is a trace statement.");
        this.logger.debug("This is a debug statement.");
        this.logger.info("This is a info statement.");
        this.logger.warn("This is a warn statement.");
        this.logger.error("This is a error statement.");

        this.logger.logLevel = currentLogLevel;
    //    console.log(`Set log level back to original level, ${this.logger.logLevel}`);
    }
}
