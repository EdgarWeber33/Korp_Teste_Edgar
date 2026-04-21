import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ProdutoListaComponent } from './components/produto-lista/produto-lista';
import { NotaFiscalListaComponent } from './components/nota-fiscal-lista/nota-fiscal-lista';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, ProdutoListaComponent, NotaFiscalListaComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class AppComponent {
  title = 'Korp_Teste_Edgar';
}
