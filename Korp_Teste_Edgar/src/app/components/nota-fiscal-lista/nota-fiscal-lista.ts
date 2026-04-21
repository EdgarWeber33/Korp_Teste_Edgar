import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NotaFiscalService, NotaFiscal, ItemNotaFiscal } from '../../services/nota-fiscal.service';
import { ProdutoService, Produto } from '../../services/produto.service';

@Component({
  selector: 'app-nota-fiscal-lista',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './nota-fiscal-lista.html',
  styleUrl: './nota-fiscal-lista.scss'
})
export class NotaFiscalListaComponent implements OnInit {
  notas: NotaFiscal[] = [];
  produtos: Produto[] = [];
  
  // Para adicionar produto à nota
  notaSelecionadaId: number = 0;
  produtoSelecionadoId: number = 0;
  quantidade: number = 1;
  
  carregando: boolean = false; // O "Spinner" que o teste pede!

  constructor(
    private notaService: NotaFiscalService,
    private produtoService: ProdutoService
  ) {}

  ngOnInit(): void {
    this.atualizarDados();
  }

  atualizarDados(): void {
    this.notaService.listarTodas().subscribe(dados => this.notas = dados);
    this.produtoService.listarTodos().subscribe(dados => this.produtos = dados);
  }

  criarNovaNota(): void {
    this.notaService.criarNota().subscribe(() => this.atualizarDados());
  }

  adicionarProduto(): void {
    if (this.notaSelecionadaId === 0 || this.produtoSelecionadoId === 0) return;
    
    const prod = this.produtos.find(p => p.id == this.produtoSelecionadoId);
    const item: ItemNotaFiscal = {
      produtoId: Number(this.produtoSelecionadoId),
      produtoDescricao: prod ? prod.descricao : '',
      quantidade: this.quantidade
    };

    this.notaService.adicionarProduto(this.notaSelecionadaId, item).subscribe(() => {
      alert('Produto adicionado!');
      this.atualizarDados();
    });
  }

  imprimir(id: number): void {
    this.carregando = true; // Ativa o indicador de processamento
    this.notaService.imprimir(id).subscribe({
      next: (res: any) => {
        alert(res.mensagem);
        this.carregando = false; // Desativa o indicador
        this.atualizarDados();
      },
      error: (err: any) => {
        alert(err.error || 'Erro ao imprimir');
        this.carregando = false;
      }
    });
  }
}
