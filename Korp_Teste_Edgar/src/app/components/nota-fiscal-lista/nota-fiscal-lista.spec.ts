import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NotaFiscalLista } from './nota-fiscal-lista';

describe('NotaFiscalLista', () => {
  let component: NotaFiscalLista;
  let fixture: ComponentFixture<NotaFiscalLista>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NotaFiscalLista],
    }).compileComponents();

    fixture = TestBed.createComponent(NotaFiscalLista);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
