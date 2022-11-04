package money_problem.unit.usecases;

import money_problem.domain.Bank;
import money_problem.domain.Currency;
import money_problem.usecases.add_exchange_rate.AddExchangeRate;
import money_problem.usecases.add_exchange_rate.AddExchangeRateUseCase;
import money_problem.usecases.ports.BankRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Nested;
import org.junit.jupiter.api.Test;

import static io.vavr.API.Some;
import static io.vavr.control.Option.none;
import static money_problem.domain.Currency.EUR;
import static money_problem.domain.Currency.USD;
import static money_problem.usecases.Success.emptySuccess;
import static money_problem.usecases.UseCaseError.error;
import static org.assertj.vavr.api.VavrAssertions.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.*;

class AddExchangeRateTest {
    private final BankRepository bankRepositoryMock = mock(BankRepository.class);
    private final AddExchangeRateUseCase addExchangeRate = new AddExchangeRateUseCase(bankRepositoryMock);

    @Nested
    class return_an_error {
        @BeforeEach
        void setup() {
            when(bankRepositoryMock.getBank()).thenReturn(none());
        }

        @Test
        void when_bank_not_setup() {
            var aValidExchangeRate = new AddExchangeRate(1, USD);
            assertError(aValidExchangeRate, "No bank defined");
        }

        @Test
        void when_exchange_rate_is_invalid() {
            var invalidExchangeRate = new AddExchangeRate(-2, USD);
            assertError(invalidExchangeRate, "Exchange rate should be greater than 0");
        }

        @Test
        void when_passing_a_rate_for_pivot_currency() {
            var pivotCurrency = EUR;
            var exchangeRateForPivot = new AddExchangeRate(0.9, pivotCurrency);

            setupBankWithPivot(pivotCurrency);

            assertError(exchangeRateForPivot, "Can not add an exchange rate for the pivot currency");
        }

        private void assertError(AddExchangeRate invalidExchangeRate, String message) {
            assertThat(addExchangeRate.invoke(invalidExchangeRate))
                    .containsOnLeft(error(message));
        }
    }

    @Nested
    class return_a_success {
        @Test
        void when_passing_a_valid_rate_for_a_currency_different_than_pivot() {
            setupBankWithPivot(EUR);
            var aValidExchangeRate = new AddExchangeRate(1, USD);

            assertThat(addExchangeRate.invoke(aValidExchangeRate))
                    .containsOnRight(emptySuccess());

            bankHasBeenSaved();
        }
    }

    private void setupBankWithPivot(Currency pivotCurrency) {
        when(bankRepositoryMock.getBank())
                .thenReturn(Some(Bank.withPivotCurrency(pivotCurrency)));
    }

    private void bankHasBeenSaved() {
        verify(bankRepositoryMock, times(1))
                .save(any(Bank.class));
    }
}
