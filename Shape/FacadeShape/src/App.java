public class App {
    public static void main(String[] args) {
        int base = 11; // Número de asteriscos na base
        int altura = 6; // Altura da pirâmide

        desenharPiramide(base, altura);
    }

    public static void desenharPiramide(int base, int altura) {
        // Verifica se a base é ímpar e >= 3
        if (base % 2 == 0 || base < 3) {
            System.out.println("A base deve ser um número ímpar e maior ou igual a 3.");
            return;
        }

        // Desenha a pirâmide linha por linha
        for (int i = 0; i < altura; i++) {
            // Desenha espaços antes dos asteriscos
            for (int j = 0; j < altura - i - 1; j++) {
                System.out.print(" ");
            }

            // Desenha o contorno da pirâmide
            for (int j = 0; j < base; j++) {
                if (j == altura - i - 1 || j == base - (altura - i) || i == altura - 1) {
                    System.out.print("*");
                } else {
                    System.out.print(" ");
                }
            }

            // Pular para a próxima linha
            System.out.println();
        }
    }
}
