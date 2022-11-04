package money_problem.usecases.setup_bank;

import money_problem.domain.Currency;
import money_problem.usecases.Command;

public record SetupBank(Currency currency) implements Command {
}
