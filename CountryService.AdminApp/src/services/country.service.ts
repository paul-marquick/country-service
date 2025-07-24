import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Country } from '../models/country';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class CountryService {

    private url: string = 'http://localhost:5581/country';
    private http: HttpClient = inject(HttpClient);

    getCountryListAsync(): Observable<Country[]> 
    {
        return this.http.get<Country[]>(this.url);
    }

    getCountryByIso2(iso2: string): Observable<Country | undefined> 
    {
        return this.http.get<Country>(`${this.url}/${iso2}`);
    }

    checkIfCountryNameExists(name: string, iso2: string | null): Observable<boolean> 
    {
        if (iso2 == null)
        {
            return this.http.get<boolean>(`${this.url}/does-country-name-exist/${name}`);
        }
        else
        {
            return this.http.get<boolean>(`${this.url}/does-country-name-exist/${name}/${iso2}`);
        }
    }
}
