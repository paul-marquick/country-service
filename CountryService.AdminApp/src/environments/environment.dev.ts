import { LogLevel } from "../logging/logLevel";

export const environment = {
    production: false,
    logLevel: LogLevel.Debug,
    apiUrl: 'http://localhost:5581',
    defaultUiSettings: {
        theme: 'dark',
        itemsPerPage: 20,
    },
};
