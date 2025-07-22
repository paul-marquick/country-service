import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountryEdit } from './country-edit';

describe('CountryEdit', () => {
  let component: CountryEdit;
  let fixture: ComponentFixture<CountryEdit>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CountryEdit]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CountryEdit);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
