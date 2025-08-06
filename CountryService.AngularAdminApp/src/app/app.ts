import { Component, isDevMode, signal } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { NgxSpinnerModule } from "ngx-spinner";

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, RouterLink, NgxSpinnerModule],
    templateUrl: './app.html',
    styleUrl: './app.css'
})
export class App {
    protected readonly title = signal('countryservice-adminapp');

    ngOnInit() 
    {
        if (isDevMode()) 
        {
            console.log('Development');
        } 
        else 
        {
            console.log('Production');
        }
    }
}
