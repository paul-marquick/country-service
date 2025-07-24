import { Component, signal } from '@angular/core';
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
}
