import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import Country from './country';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export default class CountryService {

    url = 'http://localhost:5581/country';
    private http = inject(HttpClient);

    getCountryListAsync(): Observable<Country[]> 
    {    
        return this.http.get<Country[]>(this.url);  
    }  
    
    getCountryByIso2(iso2: string): Observable<Country | undefined> 
    {    
        return this.http.get<Country>(`${this.url}/${iso2}`); 
    }
}
