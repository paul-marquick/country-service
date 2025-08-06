import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Country } from '../models/country/country';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { Logger } from '../logging/logger';

/** Sends HTTP requests and handles HTTP responses from the country rest api. */
@Injectable({
    providedIn: 'root'
})
export class CountryHttpClient {

    private logger: Logger = inject(Logger);
    private countryApiUrl: string = `${environment.apiUrl}/country`;
    private http: HttpClient = inject(HttpClient);

    /** Get country list from the web api. */
    public getCountryList(): Observable<Country[]> 
    {        
        this.logger.debug("CountryHttpClient.getCountryList.");

        return this.http.get<Country[]>(this.countryApiUrl);
    }

    /** Get a specific country. */
    public getCountryByIso2(iso2: string): Observable<Country | undefined> 
    {
        this.logger.debug("CountryHttpClient.getCountryByIso2.");

        return this.http.get<Country>(`${this.countryApiUrl}/${iso2}`);
    }

    /** Check if the country name is already in the database.
     * @remarks
     *  If an iso2 value is passed in then that row is ignored.
     */
    public checkIfCountryNameExists(name: string, iso2: string | null): Observable<boolean> 
    {
        this.logger.debug("CountryHttpClient.checkIfCountryNameExists.");

        let url: string = `${this.countryApiUrl}/does-country-name-exist/${name}`;

        if (iso2 != null)
        {
            url += `/${iso2}`;
        }
        
        return this.http.get<boolean>(url);        
    }
}
