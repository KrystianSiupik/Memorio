import 'package:dio/dio.dart';

import '../../../core/api/generated/clients/account_client.dart';
import '../../../core/api/generated/models/accept_terms_request.dart';
import '../../../core/api/generated/models/request_account_deletion_request.dart';
import '../../../core/result.dart';
import '../domain/account_repository.dart';

/// [AccountRepository] on API v1.
class HttpAccountRepository implements AccountRepository {
  const HttpAccountRepository({required this.account});

  final AccountClient account;

  @override
  Future<Result<void, AccountFailure>> acceptCurrentTerms() => _attempt(() async {
    final me = await account.getMe();
    await account.acceptTerms(body: AcceptTermsRequest(version: me.currentTermsVersion));
  });

  @override
  Future<Result<AccountOverview, AccountFailure>> loadOverview() => _attempt(() async {
    final impact = await account.getAccountDeletionImpact();
    final deletion = await account.getAccountDeletion();
    return AccountOverview(
      // The contract describes numbers as "integer or string", so the generated model has them dynamic;
      // the backend always sends numbers (D-212).
      impact: DeletionImpact(
        memories: impact.memories as int,
        photos: impact.photos as int,
        ownAlbums: impact.ownAlbums as int,
        ownAlbumsWithoutSuccessor: impact.ownAlbumsWithoutSuccessor as int,
        otherPeoplesAlbums: impact.otherPeoplesAlbums as int,
      ),
      deletionWindowEndsAt: deletion.windowEndsAt,
    );
  });

  @override
  Future<Result<void, AccountFailure>> requestDeletion({required bool consentsToAlbumExpiry}) => _attempt(
    () => account.requestAccountDeletion(
      body: RequestAccountDeletionRequest(consentsToAlbumExpiry: consentsToAlbumExpiry),
    ),
  );

  @override
  Future<Result<void, AccountFailure>> cancelDeletion() => _attempt(account.cancelAccountDeletion);

  static Future<Result<T, AccountFailure>> _attempt<T>(Future<T> Function() request) async {
    try {
      return Ok(await request());
    } on DioException catch (error) {
      return Err(_failureOf(error));
    }
  }

  static AccountFailure _failureOf(DioException error) {
    final response = error.response;
    if (response == null) {
      return const AccountOffline();
    }
    final body = response.data;
    final code = body is Map ? body['code'] : null;
    return switch ((response.statusCode, code)) {
      (401 || 403, _) => const AccountForbidden(),
      (_, 'Account.DeletionAlreadyDecided') => const DeletionAlreadyDecided(),
      (_, 'Account.AlbumWithoutSuccessor') => const AlbumWithoutSuccessor(),
      _ => const AccountServerError(),
    };
  }
}
