import { LogLevel } from "../logging/logLevel";

export const environment = {
    production: true,
    logLevel: LogLevel.Info,
    apiUrl: 'https://api.teapothub.com',
    defaultUiSettings: {
        theme: 'dark',
        itemsPerPage: 50,
    },
};
