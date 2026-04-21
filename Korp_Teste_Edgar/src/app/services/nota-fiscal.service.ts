import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ItemNotaFiscal {
  produtoId: number;
  produtoDescricao: string;
  quantidade: number;
}

export interface NotaFiscal {
  id?: number;
  numero: number;
  status: string;
  itens: ItemNotaFiscal[];
}

@Injectable({
  providedIn: 'root'
} )
export class NotaFiscalService {
  private apiUrl = 'http://localhost:5183/api/NotaFiscal';
 

  constructor(private http: HttpClient ) { }

  listarTodas(): Observable<NotaFiscal[]> {
    return this.http.get<NotaFiscal[]>(this.apiUrl );
  }

  criarNota(): Observable<NotaFiscal> {
    return this.http.post<NotaFiscal>(this.apiUrl, {} );
  }

  adicionarProduto(notaId: number, item: ItemNotaFiscal): Observable<any> {
    return this.http.post(`${this.apiUrl}/${notaId}/produto`, item );
  }

  imprimir(notaId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/${notaId}/imprimir`, {} );
  }
}
