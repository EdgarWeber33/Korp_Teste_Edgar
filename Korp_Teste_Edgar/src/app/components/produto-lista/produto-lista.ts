import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProdutoService, Produto } from '../../services/produto.service';


@Component({
  selector: 'app-produto-lista',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './produto-lista.html',
  styleUrl: './produto-lista.scss'
})
export class ProdutoListaComponent implements OnInit {
  produtos: Produto[] = [];
  novoProduto: Produto = { codigo: '', descricao: '', saldo: 0 };

  constructor(private produtoService: ProdutoService) {}

  ngOnInit(): void {
    this.listar();
  }

  listar(): void {
    this.produtoService.listarTodos().subscribe({
      next: (dados: Produto[]) => {
        this.produtos = dados;
      },
      error: (err: any) => {
        console.error('Erro ao buscar produtos', err);
      }
    });
  }

  salvar(): void {
    this.produtoService.salvar(this.novoProduto).subscribe({
      next: (resultado: Produto) => {
        alert('Produto salvo com sucesso!');
        this.listar();
        this.novoProduto = { codigo: '', descricao: '', saldo: 0 };
      },
      error: (err: any) => {
        alert('Erro ao salvar produto');
        console.error(err);
      }
    });
  }
}
