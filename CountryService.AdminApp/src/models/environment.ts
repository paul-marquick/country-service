export interface Environment {
    production: boolean;
    apiUrl: string;
    defaultUiSettings: {
        theme: string;
        itemsPerPage: number;
    };
}
