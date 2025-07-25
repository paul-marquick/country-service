import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Country } from '../models/country/country';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class CountryService {

    private countryApiUrl: string = `${environment.apiUrl}/country`;
    private http: HttpClient = inject(HttpClient);

    // Get country list from the web api.
    public getCountryListAsync(): Observable<Country[]> 
    {
        return this.http.get<Country[]>(this.countryApiUrl);
    }

    // Get a specific country.
    public getCountryByIso2(iso2: string): Observable<Country | undefined> 
    {
        return this.http.get<Country>(`${this.countryApiUrl}/${iso2}`);
    }

    // Check if the country name is already in the database. If an iso2 value is passed in then ignore that row.
    public checkIfCountryNameExists(name: string, iso2: string | null): Observable<boolean> 
    {
        let url: string = `${this.countryApiUrl}/does-country-name-exist/${name}`;

        if (iso2 != null)
        {
            url += `/${iso2}`;
        }
        
        return this.http.get<boolean>(url);        
    }
}
