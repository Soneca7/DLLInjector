#### DllInjector

Este repositório contém um programa simples em C# para injetar uma DLL em um processo remoto do Windows. A injeção de DLL é um método utilizado para carregar uma biblioteca dinâmica (DLL) dentro do espaço de memória de um processo em execução, permitindo a execução de código adicional nesse processo.

#### Funcionalidade

O programa DllInjector oferece a funcionalidade de injetar uma DLL em um processo específico do Windows. Ele suporta dois métodos de identificação do processo-alvo:

Por nome do processo: Permite injetar a DLL em um processo identificado pelo seu nome.
Por ID do processo (PID): Permite injetar a DLL em um processo identificado pelo seu ID.
Detalhes Técnicos
O código faz uso extensivo das funções da API do Windows fornecidas pelo kernel32.dll, usando P/Invoke para acessar essas funções a partir do C#. Algumas das principais funções utilizadas são:

- **OpenProcess**: Abre um handle para o processo-alvo com permissões específicas.
- **VirtualAllocEx**: Aloca espaço de memória no processo-alvo para a DLL a ser injetada.
- **WriteProcessMemory**: Escreve os bytes da DLL no espaço de memória alocado do processo-alvo.
- **GetProcAddress**: Obtém o endereço da função LoadLibraryA na kernel32.dll, que será usada para carregar a DLL no processo-alvo.
- **CreateRemoteThread**: Cria uma thread no processo-alvo para executar a função LoadLibraryA, iniciando assim o carregamento da DLL.
- **WaitForSingleObject e CloseHandle**: Aguarda a finalização da thread criada e fecha os handles dos objetos utilizados.

#### Uso
Para utilizar o DllInjector, é necessário especificar o identificador do processo (nome ou PID) e o caminho completo para a DLL que se deseja injetar. O método padrão de injeção é por nome do processo, mas isso pode ser alterado especificando InjectionMethod.ByPID.

#### Exemplo de Uso

```csharp
            string processIdentifier = guna2CustomRadioButton1.Checked ? InputProcessName.Text : InputProcessID.Text;
            string dllPath = InputPath.Text;
            bool success = false;

            if (guna2CustomRadioButton1.Checked)
            {
                success = DllInjector.InjectDll(processIdentifier, dllPath, DllInjector.InjectionMethod.ByProcessName);
            }
            else if (guna2CustomRadioButton2.Checked)
            {
                success = DllInjector.InjectDll(processIdentifier, dllPath, DllInjector.InjectionMethod.ByPID);
            }

            if (success)
            {
                MessageBox.Show("DLL injetada com sucesso!");
            }
            else
            {
                MessageBox.Show("Falha ao injetar DLL.");
            }
```

#### Observações
- **Permissões**: O programa deve ser executado com privilégios administrativos para poder injetar DLLs em outros processos.
- **Compatibilidade**: Este código foi desenvolvido e testado no ambiente Windows e pode não funcionar corretamente em outros sistemas operacionais.

#### Autores
Este código foi desenvolvido por [soneca7].

Para mais detalhes sobre o funcionamento e a implementação, consulte o código-fonte fornecido neste repositório.
