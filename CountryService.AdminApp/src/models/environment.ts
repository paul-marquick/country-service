import { LogLevel } from "../logging/logLevel";

export interface Environment {
    production: boolean;
    logLevel: LogLevel;
    apiUrl: string;
    defaultUiSettings: {
        theme: string;
        itemsPerPage: number;
    };
}
